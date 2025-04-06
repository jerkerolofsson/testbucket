using TestBucket.Contracts.Automation;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Automation.Models;
public class Pipeline : ProjectEntity
{
    public long Id { get; set; }
    public string? CiCdPipelineIdentifier { get; set; }
    public string? CiCdSystem { get; set; }
    public string? StartError { get; set; }

    /// <summary>
    /// Status of the pipeline
    /// </summary>
    public PipelineStatus Status { get; set; }

    public long? TestRunId { get; set; }
    public TestRun? TestRun { get; set; }
}
