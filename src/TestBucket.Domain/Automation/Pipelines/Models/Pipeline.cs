using TestBucket.Contracts.Automation;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Automation.Pipelines.Models;
public class Pipeline : ProjectEntity
{
    public long Id { get; set; }

    /// <summary>
    /// GUID identifier for the pipeline
    /// </summary>
    public string? Guid { get; set; }

    /// <summary>
    /// Display title
    /// </summary>
    public string? DisplayTitle { get; set; }

    /// <summary>
    /// Head commit for pipeline (SHA)
    /// </summary>
    public string? HeadCommit { get; set; }

    /// <summary>
    /// ID in remote system
    /// </summary>
    public string? CiCdPipelineIdentifier { get; set; }

    /// <summary>
    /// External system (e.g. gitlab, github)
    /// </summary>
    public string? CiCdSystem { get; set; }

    /// <summary>
    /// External project ID
    /// </summary>
    public string? CiCdProjectId { get; set; }

    /// <summary>
    /// Error message when failed to start the pipeline
    /// </summary>
    public string? StartError { get; set; }

    /// <summary>
    /// Status of the pipeline
    /// </summary>
    public PipelineStatus Status { get; set; }

    /// <summary>
    /// Pipeline duration
    /// </summary>
    public TimeSpan? Duration { get; set; }

    /// <summary>
    /// URL on external system
    /// </summary>
    public string? WebUrl { get; set; }

    // Navigation

    public long? TestRunId { get; set; }
    public TestRun? TestRun { get; set; }

    /// <summary>
    /// Pipeline jobs
    /// </summary>
    public virtual List<PipelineJob>? PipelineJobs { get; set; }
}
