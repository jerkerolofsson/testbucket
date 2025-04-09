using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using TestBucket.Contracts.Automation;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Automation.Models;
public class PipelineJob : ProjectEntity
{
    public required long Id { get; set; }

    /// <summary>
    /// Name of the job
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Id in remote system
    /// </summary>
    public string? CiCdJobIdentifier { get; set; }

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
    [Column(TypeName = "jsonb")]
    public string[]? TagList { get; set; }

    /// <summary>
    /// Reason for failure
    /// </summary>
    public string? FailureReason { get; set; }

    // Navigation

    public required long PipelineId { get; set; }

    public Pipeline? Pipeline { get; set; }

    public long? TestRunId { get; set; }
    public TestRun? TestRun { get; set; }
}
