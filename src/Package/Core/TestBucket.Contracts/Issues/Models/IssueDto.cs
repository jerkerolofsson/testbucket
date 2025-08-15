using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Issues.States;
using TestBucket.Formats.Dtos;

namespace TestBucket.Contracts.Issues.Models;
public class IssueDto
{
    /// <summary>
    /// Time when creted
    /// </summary>
    public DateTimeOffset? Created { get; set; }

    /// <summary>
    /// Time when modified
    /// </summary>
    public DateTimeOffset? Modified { get; set; }

    /// <summary>
    /// Id in external system
    /// </summary>
    public string? ExternalId { get; set; }

    /// <summary>
    /// How the ExternalId should be shown to the user
    /// </summary>
    public string? ExternalDisplayId { get; set; }

    /// <summary>
    /// Textual identifier for the external system, e.g. jira, github
    /// </summary>
    public string? ExternalSystemName { get; set; }

    /// <summary>
    /// Identifier for the external system configuration
    /// </summary>
    public long? ExternalSystemId { get; set; }

    /// <summary>
    /// Title of issue
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Description of issue
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// State
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// Mapped / known state of the issue
    /// </summary>
    public MappedIssueState MappedState { get; set; }

    /// <summary>
    /// Author/Creator
    /// </summary>
    public string? Author { get; set; }

    /// <summary>
    /// External URL
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Milestone
    /// </summary>
    public string? MilestoneName { get; set; }

    /// <summary>
    /// Type of issue
    /// </summary>
    public string? IssueType { get; set; }

    /// <summary>
    /// Labels
    /// </summary>
    public string[]? Labels { get; set; }
    public string? AssignedTo { get; set; }
    public TestTraitCollection? Traits { get; set; }
    public string? ProjectSlug { get; set; }
}
