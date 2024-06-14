using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Ametrin.Utils;
public static class TaskExtensions
{

    //by Nick Chapsas https://youtu.be/Rc4jV_3wFi4
    public static async Task<T[]> WhenAll<T>(params Task<T>[] tasks)
    {
        var allTasks = Task.WhenAll(tasks);

        try
        {
            return await allTasks;
        }
        catch(Exception)
        {
            //ignore
        }

        throw allTasks.Exception ?? throw new UnreachableException("AggregateException of all tasks was null. What the hell.");
    }

    //by Nick Chapsas https://youtu.be/Rc4jV_3wFi4
    public static TaskAwaiter<(T, T)> GetAwaiter<T>(this (Task<T>, Task<T>) tasksTuple)
    {
        async Task<(T, T)> CombineTasks()
        {
            var (task1, task2) = tasksTuple;
            await WhenAll(task1, task2);
            return (task1.Result, task2.Result);
        }

        return CombineTasks().GetAwaiter();
    }

    //by Nick Chapsas https://youtu.be/Rc4jV_3wFi4
    public static TaskAwaiter<T[]> GetAwaiter<T>(this (Task<T>, Task<T>, Task<T>) tasksTuple)
    {
        return WhenAll(tasksTuple.Item1, tasksTuple.Item2, tasksTuple.Item3).GetAwaiter();
    }
    public static TaskAwaiter<T[]> GetAwaiter<T>(this (Task<T>, Task<T>, Task<T>, Task<T>) tasksTuple)
    {
        return WhenAll(tasksTuple.Item1, tasksTuple.Item2, tasksTuple.Item3, tasksTuple.Item4).GetAwaiter();
    }
}
