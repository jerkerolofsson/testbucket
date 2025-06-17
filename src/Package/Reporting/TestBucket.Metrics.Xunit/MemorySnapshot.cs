using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Metrics.Xunit;
internal class MemorySnapshot
{
    public long WorkingSet64 { get; private set; }
    public long GcTotalMemory { get; private set; }
    public TimeSpan GcTotalPauseDuration { get; internal set; }

    public static MemorySnapshot CreateBaseline()
    {
        var snapshot = new MemorySnapshot();
        GC.Collect();
        snapshot.WorkingSet64 = Process.GetCurrentProcess().WorkingSet64;
        snapshot.GcTotalMemory = GC.GetTotalMemory(true);
        snapshot.GcTotalPauseDuration = GC.GetTotalPauseDuration();
        return snapshot;
    }
    public static MemorySnapshot CreateAfter()
    {
        var totalPauseDuration = GC.GetTotalPauseDuration();
        var workingSet64 = Process.GetCurrentProcess().WorkingSet64;
        var gcTotalMemory = GC.GetTotalMemory(true);
        GC.Collect();

        var snapshot = new MemorySnapshot();
        snapshot.WorkingSet64 = workingSet64;
        snapshot.GcTotalMemory = gcTotalMemory;
        snapshot.GcTotalPauseDuration = totalPauseDuration;
        return snapshot;
    }
}
