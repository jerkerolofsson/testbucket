using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Metrics.Xunit;
public class MemoryTest
{
    public static async ValueTask RunAsync(Func<ValueTask> test, int iterationsWarmup = 10000, int iterationsTest = 10000)
    {
        // Warm-up
        for (int i = 0; i < iterationsWarmup; i++)
        {
            await test();
        }

        IncludeDiagnosticsAttribute.Current?.CreateMemoryBaseline();

        // Main
        for (int i = 0; i < iterationsTest; i++)
        {
            await test();
        }

        IncludeDiagnosticsAttribute.Current?.ReportMemory();
    }
    public static void Run(Action test, int iterationsWarmup = 100000, int iterationsTest = 10000)
    {
        // Warm-up
        for (int i = 0; i < iterationsWarmup; i++)
        {
            test();
        }

        IncludeDiagnosticsAttribute.Current?.CreateMemoryBaseline();

        // Main
        for (int i = 0; i < iterationsTest; i++)
        {
            test();
        }

        IncludeDiagnosticsAttribute.Current?.ReportMemory();
    }

}
