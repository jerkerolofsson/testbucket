using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Milestones.Specifications;
using TestBucket.Domain.Shared.Specifications;

using static System.Net.Mime.MediaTypeNames;

namespace TestBucket.Domain.Milestones;
internal class MilestoneManager : IMilestoneManager
{
    private readonly IMilestoneRepository _repository;
    private readonly TimeProvider _timeProvider;

    public MilestoneManager(IMilestoneRepository repository, TimeProvider timeProvider)
    {
        _repository = repository;
        _timeProvider = timeProvider;
    }

    public async Task UpdateMilestoneAsync(ClaimsPrincipal principal, Milestone milestone)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Issue, PermissionLevel.Write);
        principal.ThrowIfEntityTenantIsDifferent(milestone);
        milestone.Modified = DateTimeOffset.UtcNow;
        milestone.ModifiedBy = principal.Identity?.Name;
        await _repository.UpdateMilestoneAsync(milestone);
    }

    public async Task<IReadOnlyList<Milestone>> GetMilestonesAsync(ClaimsPrincipal principal, long projectId)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Issue, PermissionLevel.Read);
        var filters = new List<FilterSpecification<Milestone>>
        {
            new FilterByTenant<Milestone>(principal.GetTenantIdOrThrow()),
            new FilterByProject<Milestone>(projectId)
        };
        return await _repository.GetMilestonesAsync(filters);
    }

    public async Task AddMilestoneAsync(ClaimsPrincipal principal, Milestone milestone)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Issue, PermissionLevel.Write);

        milestone.TenantId = principal.GetTenantIdOrThrow();
        milestone.Created = DateTimeOffset.UtcNow;
        milestone.Modified = DateTimeOffset.UtcNow;
        milestone.CreatedBy = principal.Identity?.Name;
        milestone.ModifiedBy = principal.Identity?.Name;
        await _repository.AddMilestoneAsync(milestone);
    }


    public async Task<Milestone?> GetMilestoneByExternalIdAsync(ClaimsPrincipal principal, long projectId, string systemName, string externalId)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Issue, PermissionLevel.Read);
        var filters = new List<FilterSpecification<Milestone>>
        {
            new FilterByTenant<Milestone>(principal.GetTenantIdOrThrow()),
            new FilterByProject<Milestone>(projectId),
            new FilterMilestoneByExternalId(systemName, externalId),
        };
        var result = await _repository.GetMilestonesAsync(filters);
        return result.FirstOrDefault();
    }


    public async Task<Milestone?> GetMilestoneByNameAsync(ClaimsPrincipal principal, long projectId, string name)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Issue, PermissionLevel.Read);
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
        principal.ThrowIfNoPermission(PermissionEntityType.Issue, PermissionLevel.Read);
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
        principal.ThrowIfNoPermission(PermissionEntityType.Issue, PermissionLevel.Delete);
        principal.ThrowIfEntityTenantIsDifferent(milestone);

        await _repository.DeleteMilestoneByIdAsync(milestone.Id);
    }

    public async Task<Milestone?> GetCurrentMilestoneAsync(ClaimsPrincipal principal, long projectId)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Issue, PermissionLevel.Read);

        var filters = new List<FilterSpecification<Milestone>>
        {
            new FilterByTenant<Milestone>(principal.GetTenantIdOrThrow()),
            new FilterByProject<Milestone>(projectId),
            new FilterMilestonesByCurrent(_timeProvider.GetUtcNow())
        };
        var result = await _repository.GetMilestonesAsync(filters);
        return result.FirstOrDefault();
    }
}
