using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Issues.Models;

namespace TestBucket.Domain.Issues.Models;
public class Milestone : ProjectEntity
{
    /// <summary>
    /// DB ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Id in external system
    /// </summary>
    public string? ExternalId { get; set; }

    /// <summary>
    /// Textual identifier for the external system, e.g. jira, github
    /// </summary>
    public string? ExternalSystemName { get; set; }

    /// <summary>
    /// Identifier for the external system configuration
    /// </summary>
    public long? ExternalSystemId { get; set; }

    /// <summary>
    /// Title of milestone
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Description of milestone
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Color for visualizastion of milestone
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// State of the milestone
    /// </summary>
    public MilestoneState State { get; set; }

    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
}
