using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Automation;

/// <summary>
/// Gitlab: Pipeline
/// Github Actions: Workflow Run
/// </summary>
public class PipelineDto
{
    /// <summary>
    /// Flag that indicates if the pipeline is completed
    /// </summary>
    public bool IsCompleted => Status is PipelineStatus.Failed or PipelineStatus.Completed or PipelineStatus.Success or PipelineStatus.Error;

    /// <summary>
    /// Pipeline status
    /// </summary>
    public PipelineStatus Status { get; set; }

    /// <summary>
    /// Duration of the pipeline
    /// </summary>
    public TimeSpan? Duration { get; set; }

    /// <summary>
    /// Any error related to the pipeline
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Web URL for the pipeline in external system
    /// </summary>
    public string? WebUrl { get; set; }

    /// <summary>
    /// Jobs in the pipeline
    /// </summary>
    public List<PipelineJobDto> Jobs { get; set; } = [];

    /// <summary>
    /// External project ID in remote system
    /// </summary>
    public string? CiCdProjectId { get; set; }

    /// <summary>
    /// ID in exsternal system
    /// </summary>
    public string? CiCdPipelineIdentifier { get; set; }
}
