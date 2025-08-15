namespace TestBucket.Contracts.Issues.Models;
public class MilestoneDto
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public DateTimeOffset? DueDate { get; set; }
    public long Id { get; set; }
    public string? Color { get; set; }
    public MilestoneState State { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public string? ExternalId { get; set; }
    public string? ExternalSystemName { get; set; }
    public long? ExternalSystemId { get; set; }
    public string? ProjectSlug { get; set; }
}
