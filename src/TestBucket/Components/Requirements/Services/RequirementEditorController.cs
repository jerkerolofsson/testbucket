
using TestBucket.Components.Tests.Services;
using TestBucket.Domain.Files.Models;
using TestBucket.Domain.Requirements;
using TestBucket.Domain.Requirements.Import;
using TestBucket.Domain.Requirements.Models;

namespace TestBucket.Components.Requirements.Services;


internal interface IRequirementObserver
{
    Task OnSpecificationCreatedAsync(RequirementSpecification spec);
    Task OnSpecificationDeletedAsync(RequirementSpecification spec);
    Task OnSpecificationSavedAsync(RequirementSpecification spec);
}
internal class RequirementEditorController : TenantBaseService
{
    private readonly List<IRequirementObserver> _requirementObservers = new();
    private readonly IRequirementImporter _importer;
    private readonly IRequirementManager _manager;
    public RequirementEditorController(
        AuthenticationStateProvider authenticationStateProvider, 
        IRequirementImporter importer, 
        IRequirementManager manager) : base(authenticationStateProvider)
    {
        _importer = importer;
        _manager = manager;
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

    public async Task ExtractRequirementsFromSpecificationAsync(RequirementSpecification specification, CancellationToken cancellationToken = default)
    {
        var requirements = await _importer.ExtractRequirementsAsync(specification, cancellationToken);

        var principal = await GetUserClaimsPrincipalAsync();
        foreach (var requirement in requirements)
        {
            cancellationToken.ThrowIfCancellationRequested();

            requirement.TestProjectId = specification.TestProjectId;
            requirement.TeamId = specification.TeamId;
            requirement.RequirementSpecificationId = specification.Id;
            await _manager.AddRequirementAsync(principal, requirement);
        }
    }

    public async Task<RequirementSpecification?> ImportAsync(long? teamId, long? testProjectId, FileResource fileResource)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        var specification = await _importer.ImportFileAsync(principal, teamId, testProjectId, fileResource);

        if (specification is not null)
        {
            await AddRequirementSpecificationAsync(specification);
        }

        return specification;
    }
    public async Task AddRequirementSpecificationAsync(RequirementSpecification specification)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _manager.AddRequirementSpecificationAsync(principal, specification);

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

        var principal = await GetUserClaimsPrincipalAsync();
        await _manager.UpdateRequirementSpecificationAsync(principal, specification);

        foreach(var observer in _requirementObservers)
        {
            await observer.OnSpecificationSavedAsync(specification);
        }
    }

    public async Task<PagedResult<RequirementSpecification>> GetRequirementSpecificationsAsync(long? teamId, long? projectId, int offset = 0, int count = 100)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _manager.SearchRequirementSpecificationsAsync(principal, new SearchQuery
        {
            TeamId = teamId,
            ProjectId = projectId,
            Offset = offset,
            Count = count
        });
    }


}
