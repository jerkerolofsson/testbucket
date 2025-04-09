using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Automation;
public class PipelineJobDto
{
    /// <summary>
    /// Time when created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Time when started
    /// </summary>
    public DateTimeOffset StartedAt { get; set; }

    /// <summary>
    /// Time when completed
    /// </summary>
    public DateTimeOffset FinishedAt { get; set; }

    /// <summary>
    /// Pipeline stage
    /// </summary>
    public string? Stage { get; set; }

    /// <summary>
    /// Coverage
    /// </summary>
    public double? Coverage { get; set; }

    /// <summary>
    /// Job status
    /// </summary>
    public PipelineJobStatus? Status { get; set; }

    /// <summary>
    /// Does the job allow failure
    /// </summary>
    public bool? AllowFailure { get; set; }

    /// <summary>
    /// Web URL for the job
    /// </summary>
    public string? WebUrl { get; set; }

    /// <summary>
    /// Duration for the job
    /// </summary>
    public TimeSpan? Duration { get; set; }

    /// <summary>
    /// Time the job is/was queued
    /// </summary>
    public TimeSpan? QueuedDuration { get; set; }

    /// <summary>
    /// List of tags
    /// </summary>
    public string[]? TagList { get; set; }

    /// <summary>
    /// Reason for failure
    /// </summary>
    public string? FailureReason { get; set; }
    public string Name { get; set; }
    public string CiCdJobIdentifier { get; set; }
}
