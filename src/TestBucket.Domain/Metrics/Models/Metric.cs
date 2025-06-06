using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Metrics.Models;
public class Metric : ProjectEntity
{
    /// <summary>
    /// DB ID
    /// </summary>
    public long Id { get; set; }

    public required string MeterName { get; set; }
    public required string Name { get; set; }
    public required double Value { get; set; }
    public string? Unit { get; set; }

    // Navigation

    public long TestCaseRunId { get; set; }
    public TestCaseRun? TestCaseRun { get; set; }
}
