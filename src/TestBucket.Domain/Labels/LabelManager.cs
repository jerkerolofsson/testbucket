using TestBucket.Domain.Labels.Models;
using TestBucket.Domain.Labels.Specifications;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Labels;
internal class LabelManager : ILabelManager
{
    private readonly ILabelRepository _repository;
    private readonly TimeProvider _timeProvider;

    public LabelManager(ILabelRepository repository, TimeProvider timeProvider)
    {
        _repository = repository;
        _timeProvider = timeProvider;
    }

    public async Task UpdateLabelAsync(ClaimsPrincipal principal, Label Label)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Issue, PermissionLevel.Write);
        principal.ThrowIfEntityTenantIsDifferent(Label);
        Label.Modified = _timeProvider.GetUtcNow();
        Label.ModifiedBy = principal.Identity?.Name;
        await _repository.UpdateLabelAsync(Label);
    }

    public async Task<IReadOnlyList<Label>> GetLabelsAsync(ClaimsPrincipal principal, long projectId)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Issue, PermissionLevel.Read);
        var filters = new List<FilterSpecification<Label>>
        {
            new FilterByTenant<Label>(principal.GetTenantIdOrThrow()),
            new FilterByProject<Label>(projectId)
        };
        return await _repository.GetLabelsAsync(filters);
    }

    public async Task AddLabelAsync(ClaimsPrincipal principal, Label Label)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Issue, PermissionLevel.Write);

        Label.TenantId = principal.GetTenantIdOrThrow();
        Label.Created = _timeProvider.GetUtcNow();
        Label.Modified = _timeProvider.GetUtcNow();
        Label.CreatedBy = principal.Identity?.Name;
        Label.ModifiedBy = principal.Identity?.Name;
        await _repository.AddLabelAsync(Label);
    }

    public async Task<Label?> GetLabelByNameAsync(ClaimsPrincipal principal, long projectId, string name)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Issue, PermissionLevel.Read);
        var filters = new List<FilterSpecification<Label>>
        {
            new FilterByTenant<Label>(principal.GetTenantIdOrThrow()),
            new FilterByProject<Label>(projectId),
            new FindByLabelName(name),
        };
        var result = await _repository.GetLabelsAsync(filters);
        return result.FirstOrDefault();
    }

    public async Task<IReadOnlyList<Label>> SearchLabelsAsync(ClaimsPrincipal principal, long projectId, string text, int offset, int count)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Issue, PermissionLevel.Read);
        var filters = new List<FilterSpecification<Label>>
        {
            new FilterByTenant<Label>(principal.GetTenantIdOrThrow()),
            new FilterByProject<Label>(projectId),
            new FindByTitleContains(text)
        };
        var result = await _repository.GetLabelsAsync(filters);
        return result.Skip(offset).Take(count).ToList();
    }

    public async Task DeleteAsync(ClaimsPrincipal principal, Label Label)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Issue, PermissionLevel.Delete);
        principal.ThrowIfEntityTenantIsDifferent(Label);

        await _repository.DeleteLabelByIdAsync(Label.Id);
    }
}
