﻿using System.Collections.Frozen;

namespace Ametrin.Utils;

public static class LinqExtensions
{
    public static TimeSpan Sum<T>(this IEnumerable<T> values, Func<T, TimeSpan> selector)
        => TimeSpan.FromTicks(values.Sum(v => selector(v).Ticks));
    public static TimeSpan Sum(this IEnumerable<TimeSpan> values)
        => TimeSpan.FromTicks(values.Sum(static timeSpan => timeSpan.Ticks));

    public static string Dump<T>(this IEnumerable<T> values, char separator, string format) where T : IFormattable
        => values.Select(t => t.ToString(format, null)).Dump(separator);
    public static string Dump<T>(this IEnumerable<T> values, string separator)
        => string.Join(separator, values);
    public static string Dump<T>(this IEnumerable<T> values, char separator)
        => string.Join(separator, values);

    public static IEnumerable<string> SelectDuplicates(this IEnumerable<string> values)
        => values.CountBy(x => x).Where(g => g.Value > 1).Select(g => g.Key);

    public static FrozenDictionary<TKey, TResult> ToFrozenDictionary<TKey, TResult, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source, Func<TValue, TResult> map) where TKey : notnull
        => source.Select(pair => new KeyValuePair<TKey, TResult>(pair.Key, map(pair.Value))).ToFrozenDictionary();

    public static void Consume<T>(this IEnumerable<T> values, Action<T> action)
    {
        foreach (var value in values)
        {
            action(value);
        }
    }
    public static void Consume<T>(this IEnumerable<T> values, Action<int, T> action)
    {
        var count = 0;
        foreach (var value in values)
        {
            action(count, value);
            count++;
        }
    }

    public static void Consume<T>(this IEnumerable<T> values, Action<T> action, IProgress<int> progress)
        => values.Consume((idx, value) => action(value), progress);
    public static void Consume<T>(this IEnumerable<T> values, Action<int, T> action, IProgress<int> progress)
    {
        var count = 0;
        foreach (var value in values)
        {
            progress.Report(count);
            action(count, value);
            count++;
        }
    }

    public static void Consume<T>(this IEnumerable<T> enumerable, Action<T> action, IProgress<float> progress)
        => enumerable.Consume((idx, value) => action(value), progress);
    public static void Consume<T>(this IEnumerable<T> enumerable, Action<int, T> action, IProgress<float> progress)
    {
        var values = enumerable switch
        {
            ICollection<T> collection => collection,
            _ => [.. enumerable],
        };

        float total = values.Count;
        for (var i = 0; i < values.Count; i++)
        {
            progress.Report(i / total);
            action(i, values.ElementAt(i));
        }
    }

    public static void Consume<T>(this IEnumerable<IEnumerable<T>> enumerable, Action<T> action, IProgress<float> progress)
    {
        var values = enumerable switch
        {
            ICollection<ICollection<T>> collection => collection,
            IEnumerable<ICollection<T>> collection => collection.ToArray(),
            _ => enumerable.Select(e => e.ToArray()).ToArray(),
        };
        var total = values.Sum(s => s.Count);
        var count = 0;
        foreach (var subCollection in values)
        {
            foreach (var value in subCollection)
            {
                progress.Report(count / total);
                action(value);
                count++;
            }
        }
    }
}
