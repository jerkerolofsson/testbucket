using System.Collections.Concurrent;

namespace TestBucket.Data.Migrations;
public class MigrationReadyWaiter
{
    public static bool IsReady { get; set; } = false;
    private static readonly ManualResetEventSlim _ready = new ManualResetEventSlim(false);

    private static readonly ConcurrentBag<Func<Task>> _callbacksReady = [];

    public static async Task AddWhenReadyAsync(Func<Task> action)
    {
        if(IsReady)
        {
            await action();
            return;
        }
        _callbacksReady.Add(action);
    }

    public static void Wait()
    {
        _ready.Wait();
    }

    internal static async Task SetReadyAsync()
    {
        IsReady = true;

        foreach(var action in _callbacksReady)
        {
            await action();
        }

        _callbacksReady.Clear();

        _ready.Set();
    }
}
