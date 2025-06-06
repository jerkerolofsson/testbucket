namespace TestBucket.Traits.Core.Metrics;

/// <summary>
/// A measurement or numerical result that can be used to track performance, progress, or other quantifiable aspects of a system or process.
/// </summary>
/// <param name="MeterName"></param>
/// <param name="Name"></param>
/// <param name="Value"></param>
/// <param name="Unit"></param>
public record class TestResultMetric(string MeterName, string Name, double Value, string? Unit)
{
    public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;

    public static TestResultMetric Create(string MeterName, string Name, TimeSpan timespan)
    {
        var ms = timespan.TotalMilliseconds;
        return new TestResultMetric(MeterName, Name, ms, "ms");
    }
}
