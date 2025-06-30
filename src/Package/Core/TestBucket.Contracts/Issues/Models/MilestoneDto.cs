namespace TestBucket.Contracts.Issues.Models;
public class MilestoneDto
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public DateTimeOffset? DueDate { get; set; }
    public long Id { get; set; }

    /// <summary>
    /// True if this is the next milestone
    /// </summary>
    public bool IsNext { get; set; }

    /// <summary>
    /// True if the milestone is open, i.e. not completed
    /// </summary>
    public bool IsOpen { get; set; }

    /// <summary>
    /// True if the due date is in the past
    /// </summary>
    public bool IsOverdue { get; set; }
}
