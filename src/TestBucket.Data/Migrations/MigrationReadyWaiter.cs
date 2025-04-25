using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Data.Migrations;
public class MigrationReadyWaiter
{
    public static bool IsReady { get; set; } = false;
    private static readonly ManualResetEventSlim _ready = new ManualResetEventSlim(false);
    public static void Wait()
    {
        _ready.Wait();
    }

    internal static void SetReady()
    {
        IsReady = true;
        _ready.Set();
    }
}
