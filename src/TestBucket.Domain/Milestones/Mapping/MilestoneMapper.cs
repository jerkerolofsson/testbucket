using TestBucket.Contracts.Issues.Models;
using TestBucket.Domain.Issues.Models;

namespace TestBucket.Domain.Milestones.Mapping;
internal static class MilestoneMapper
{
    public static MilestoneDto ToDto(this Milestone milestone)
    {
        return new MilestoneDto
        {
            Title = milestone.Title ?? "",
            Description = milestone.Description,
            StartDate = milestone.StartDate,
            DueDate = milestone.EndDate,
            Id = milestone.Id,
            State = milestone.State,
            Color = milestone.Color,
            ExternalId = milestone.ExternalId,
            ExternalSystemName = milestone.ExternalSystemName,
            ExternalSystemId = milestone.ExternalSystemId,
        };
    }
}
