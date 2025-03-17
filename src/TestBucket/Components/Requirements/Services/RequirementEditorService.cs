
using TestBucket.Components.Tests.Services;
using TestBucket.Domain.Requirements;
using TestBucket.Domain.Requirements.Models;

namespace TestBucket.Components.Requirements.Services;


internal interface IRequirementObserver
{
    Task OnSpecificationCreatedAsync(RequirementSpecification spec);
    Task OnSpecificationDeletedAsync(RequirementSpecification spec);
    Task OnSpecificationSavedAsync(RequirementSpecification spec);
}
internal class RequirementEditorService : TenantBaseService
{
    private readonly List<IRequirementObserver> _requirementObservers = new();
    private readonly IRequirementRepository _repository;

    public RequirementEditorService(
        IRequirementRepository repository,
        AuthenticationStateProvider authenticationStateProvider) : base(authenticationStateProvider)
    {
        _repository = repository;
    }

    /// <summary>
    /// Adds an observer
    /// </summary>
    /// <param name="listener"></param>
    public void AddObserver(IRequirementObserver observer) => _requirementObservers.Add(observer);

    /// <summary>
    /// Removes an observer
    /// </summary>
    /// <param name="observer"></param>
    public void RemoveObserver(IRequirementObserver observer) => _requirementObservers.Remove(observer);

    public async Task AddRequirementSpecificationAsync(RequirementSpecification specification)
    {
        var tenantId = await GetTenantIdAsync();
        specification.TenantId = tenantId;
        await _repository.AddRequirementSpecificationAsync(tenantId, specification);

        foreach (var observer in _requirementObservers)
        {
            await observer.OnSpecificationCreatedAsync(specification);
        }
    }
    public async Task SaveRequirementSpecificationAsync(RequirementSpecification specification)
    {
        var tenantId = await GetTenantIdAsync();
        if(specification.TenantId != specification.TenantId)
        {
            throw new InvalidOperationException("Tenant ID mismatch");
        }
        await _repository.UpdateRequirementSpecificationAsync(specification);

        foreach(var observer in _requirementObservers)
        {
            await observer.OnSpecificationSavedAsync(specification);
        }
    }

    public async Task<PagedResult<RequirementSpecification>> GetRequirementSpecificationsAsync(long? teamId, long? projectId, int offset = 0, int count = 100)
    {
        var tenantId = await GetTenantIdAsync();
        return await _repository.SearchRequirementSpecificationsAsync(tenantId, new SearchQuery
        {
            TeamId = teamId,
            ProjectId = projectId,
            Offset = offset,
            Count = count
        });
    }


}
