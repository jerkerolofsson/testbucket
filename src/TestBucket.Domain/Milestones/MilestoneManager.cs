using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Milestones.Specifications;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Milestones;
internal class MilestoneManager : IMilestoneManager
{
    private readonly IMilestoneRepository _repository;

    public MilestoneManager(IMilestoneRepository repository)
    {
        _repository = repository;
    }

    public async Task UpdateMilestoneAsync(ClaimsPrincipal principal, Milestone milestone)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Architecture, PermissionLevel.Write);
        principal.ThrowIfEntityTenantIsDifferent(milestone);
        milestone.Modified = DateTimeOffset.UtcNow;
        milestone.ModifiedBy = principal.Identity?.Name;
        await _repository.UpdateMilestoneAsync(milestone);
    }

    public async Task<IReadOnlyList<Milestone>> GetMilestonesAsync(ClaimsPrincipal principal, long projectId)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Architecture, PermissionLevel.Write);
        var filters = new List<FilterSpecification<Milestone>>
        {
            new FilterByTenant<Milestone>(principal.GetTenantIdOrThrow()),
            new FilterByProject<Milestone>(projectId)
        };
        return await _repository.GetMilestonesAsync(filters);
    }

    public async Task AddMilestoneAsync(ClaimsPrincipal principal, Milestone milestone)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Architecture, PermissionLevel.Write);

        milestone.TenantId = principal.GetTenantIdOrThrow();
        milestone.Created = DateTimeOffset.UtcNow;
        milestone.Modified = DateTimeOffset.UtcNow;
        milestone.CreatedBy = principal.Identity?.Name;
        milestone.ModifiedBy = principal.Identity?.Name;
        await _repository.AddMilestoneAsync(milestone);
    }

    public async Task<Milestone?> GetMilestoneByNameAsync(ClaimsPrincipal principal, long projectId, string name)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Architecture, PermissionLevel.Write);
        var filters = new List<FilterSpecification<Milestone>>
        {
            new FilterByTenant<Milestone>(principal.GetTenantIdOrThrow()),
            new FilterByProject<Milestone>(projectId),
            new FilterByTitle(name),
        };
        var result = await _repository.GetMilestonesAsync(filters);
        return result.FirstOrDefault();
    }

    public async Task<IReadOnlyList<Milestone>> SearchMilestonesAsync(ClaimsPrincipal principal, long projectId, string text, int offset, int count)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Architecture, PermissionLevel.Write);
        var filters = new List<FilterSpecification<Milestone>>
        {
            new FilterByTenant<Milestone>(principal.GetTenantIdOrThrow()),
            new FilterByProject<Milestone>(projectId),
            new SearchMilestones(text)
        };
        var result = await _repository.GetMilestonesAsync(filters);
        return result.Skip(offset).Take(count).ToList();
    }

    public async Task DeleteAsync(ClaimsPrincipal principal, Milestone milestone)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Architecture, PermissionLevel.Delete);
        principal.ThrowIfEntityTenantIsDifferent(milestone);

        await _repository.DeleteMilestoneByIdAsync(milestone.Id);
    }
}
