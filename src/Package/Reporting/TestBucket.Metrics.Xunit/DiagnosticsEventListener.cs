using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.Tracing;

using TestBucket.Traits.Core.Metrics;

namespace TestBucket.Metrics.Xunit;

internal class DiagnosticsEventListener : EventListener
{
    private MemorySnapshot? _memorySnapshot;
    private readonly List<TestResultMetric> _memoryMetrics = [];
    public DiagnosticsEventListener()
    {
    }

    internal void CreateMemoryBaseline()
    {
        _memorySnapshot = MemorySnapshot.CreateBaseline();
    }

    internal void ReportMemory()
    {
        if(_memorySnapshot is null)
        {
            return;
        }
        // As we force a collection of the GC here, we log the total pause duration before..
        var memorySnapshot = MemorySnapshot.CreateAfter();

        var heapDelta = memorySnapshot.GcTotalMemory - _memorySnapshot.GcTotalMemory;
        var workingSet64Delta = memorySnapshot.WorkingSet64 - _memorySnapshot.WorkingSet64;

        if (workingSet64Delta > 0)
        {
            _memoryMetrics.Add(new TestResultMetric("process", "working_set_delta", workingSet64Delta, "B"));
        }
        if (_memorySnapshot.WorkingSet64 > 0)
        {
            _memoryMetrics.Add(new TestResultMetric("process", "working_set", _memorySnapshot.WorkingSet64, "B"));
        }

        if (memorySnapshot.GcTotalMemory > 0)
        {
            _memoryMetrics.Add(new TestResultMetric("dotnet", "gc_delta", heapDelta, "B"));
            _memoryMetrics.Add(new TestResultMetric("dotnet", "gc_total_memory", memorySnapshot.GcTotalMemory, "B"));
        }
        if (_memorySnapshot.GcTotalMemory > 0)
        {
            _memoryMetrics.Add(new TestResultMetric("dotnet", "gc_total_memory_before", _memorySnapshot.GcTotalMemory, "B"));
        }

        var timeInGc = memorySnapshot.GcTotalPauseDuration.TotalMilliseconds - _memorySnapshot.GcTotalPauseDuration.TotalMilliseconds;
        if (timeInGc > 0)
        {
            _memoryMetrics.Add(new TestResultMetric("dotnet", "time_in_gc", timeInGc, "ms"));
        }
    }

    protected override void OnEventSourceCreated(EventSource eventSource)
    {
        if (eventSource.Name == "System.Runtime")
        {
            EnableEvents(eventSource, EventLevel.Informational, EventKeywords.All, new Dictionary<string, string?>
            {
                ["EventCounterIntervalSec"] = "1"
            });
        }
        if (eventSource.Name == "Npgsql")
        {
            EnableEvents(eventSource, EventLevel.Verbose, EventKeywords.All, new Dictionary<string, string?>
            {
                ["EventCounterIntervalSec"] = "1"
            });
        }
    }

    internal IEnumerable<TestResultMetric> CreateMetrics()
    {
        foreach(var metric in _memoryMetrics)
        {
            yield return metric;
        }
        if (_npgsqlTotalCommands.Value > 0)
        {
            yield return new TestResultMetric("npgsql", "total_commands", _npgsqlTotalCommands.Value, "#");
        }
        if (_meanCpuUsage > 0)
        {
            yield return new TestResultMetric("dotnet", "cpu_usage.mean", _meanCpuUsage, "%");
        }
        if (_exceptionCount.Value > 0)
        {
            yield return new TestResultMetric("dotnet", "dotnet.exceptions", _exceptionCount.Value, "#");
        }

    }

    private readonly RelativeCounter<int> _exceptionCount = new();
    private readonly RelativeCounter<int> _npgsqlTotalCommands = new();
    private double _meanCpuUsage = 0;

    protected override void OnEventWritten(EventWrittenEventArgs eventData)
    {
        if (eventData.EventName == "EventCounters" && eventData.Payload?.Count > 0)
        {
            foreach (IDictionary<string, object>? payload in eventData.Payload)
            {
                if (payload != null && payload.TryGetValue("Name", out var nameValue))
                {
                    var name = nameValue?.ToString();
                    if (name is not null)
                    {
                        if (eventData.EventSource.Name == "System.Runtime")
                        {
                            if (name == "exception-count")
                            {
                                if (payload.TryGetValue("Increment", out var value) && value is double inc)
                                {
                                    _exceptionCount.Increment((int)inc);
                                }
                            }
                            if (name == "cpu-usage")
                            {
                                if (payload.TryGetValue("Mean", out var value) && value is double cpuUsage)
                                {
                                    _meanCpuUsage = cpuUsage;
                                }
                            }
                        }

                        if (eventData.EventSource.Name == "Npgsql")
                        {
                            if (name == "total-commands")
                            {
                                if (payload.TryGetValue("Count", out var countValue) && countValue is int count)
                                {
                                    _npgsqlTotalCommands.Set(count);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}