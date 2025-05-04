
using TestBucket.Domain.Issues.Models;

namespace TestBucket.Domain.Milestones;
public interface IMilestoneManager
{
    Task<Milestone?> GetMilestoneByNameAsync(ClaimsPrincipal principal, long projectId, string name);

    Task AddMilestoneAsync(ClaimsPrincipal principal, Milestone milestone);
    Task<IReadOnlyList<Milestone>> GetMilestonesAsync(ClaimsPrincipal principal, long projectId);
    Task UpdateMilestoneAsync(ClaimsPrincipal principal, Milestone milestone);
}