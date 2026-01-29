using System.Collections.Frozen;

namespace Ametrin.Utils;

public static class LinqExtensions
{
    extension<T>(IEnumerable<T> source)
    {
        public TimeSpan Sum(Func<T, TimeSpan> selector)
            => TimeSpan.FromTicks(source.Sum(v => selector(v).Ticks));

        public string Dump(string separator)
            => string.Join(separator, source);
        public string Dump(char separator)
            => string.Join(separator, source);

        public void ForEach(Action<T> action)
        {
            foreach (var value in source)
            {
                action(value);
            }
        }
        public void ForEach(Action<int, T> action)
        {
            var count = 0;
            foreach (var value in source)
            {
                action(count, value);
                count++;
            }
        }

        public void ForEach(Action<T> action, IProgress<float> progress)
            => source.ForEach((idx, value) => action(value), progress);
        public void ForEach(Action<int, T> action, IProgress<float> progress)
        {
            var values = source switch
            {
                ICollection<T> collection => collection,
                _ => [.. source],
            };

            float total = values.Count;
            for (var i = 0; i < values.Count; i++)
            {
                progress.Report(i / total);
                action(i, values.ElementAt(i));
            }
        }

        public void ForEach(Action<T> action, IProgress<int> progress)
            => source.ForEach((idx, value) => action(value), progress);
        public void ForEach(Action<int, T> action, IProgress<int> progress)
        {
            var count = 0;
            foreach (var value in source)
            {
                progress.Report(count);
                action(count, value);
                count++;
            }
        }
    }

    extension<T>(IEnumerable<T> source) where T : IFormattable
    {
        public string Dump(char separator, string format) => source.Select(t => t.ToString(format, null)).Dump(separator);
    }

    extension(IEnumerable<string> source)
    {
        public IEnumerable<string> SelectDuplicates()
            => source.CountBy(x => x).Where(g => g.Value > 1).Select(g => g.Key);
    }

    public static FrozenDictionary<TKey, TResult> ToFrozenDictionary<TKey, TResult, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source, Func<TValue, TResult> selector) where TKey : notnull
        => source.Select(pair => KeyValuePair.Create(pair.Key, selector(pair.Value))).ToFrozenDictionary();

    extension<T>(IEnumerable<IEnumerable<T>> source)
    {
        [Obsolete]
        public void Consume(Action<T> action, IProgress<float> progress)
        {
            var values = source switch
            {
                ICollection<ICollection<T>> collection => collection,
                IEnumerable<ICollection<T>> collection => [.. collection],
                _ => [.. source.Select(e => e.ToArray())],
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
}
