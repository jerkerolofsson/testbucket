namespace TestBucket.Domain.Labels.Models;
public class Label : ProjectEntity
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
    /// Title of label
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Description of label
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Label color
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// ReadOnly label (from external system that cannot be modified)
    /// </summary>
    public bool ReadOnly { get; set; }
}
