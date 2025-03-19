using OneOf;

using TestBucket.Contracts.Testing.Models;
using TestBucket.Domain.Errors;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Projects;
using TestBucket.Domain.States;

namespace TestBucket.Components.Projects;

internal class ProjectService : TenantBaseService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserPreferencesManager _userPreferencesService;
    private readonly IStateService _stateService;

    public ProjectService(
        IProjectRepository projectRepository,
        IUserPreferencesManager userPreferencesService,
        AuthenticationStateProvider authenticationStateProvider,
        IStateService stateService) : base(authenticationStateProvider)
    {
        _projectRepository = projectRepository;
        _userPreferencesService = userPreferencesService;
        _stateService = stateService;
    }

    public async Task SetActiveProjectAsync(TestProject? project)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        var preferences = await _userPreferencesService.LoadUserPreferencesAsync(principal);
        if(preferences is not null)
        {
            preferences.ActiveProjectId = project?.Id;
            await _userPreferencesService.SaveUserPreferencesAsync(principal, preferences);    
        }
    }

    public async Task<TestState[]> GetStatesAsync(long? projectId)
    {
        var tenantId = await GetTenantIdAsync();
        if(projectId is null)
        {
            return _stateService.GetDefaultStates();
        }
        return await _stateService.GetProjectStatesAsync(tenantId, projectId.Value);
    }

    public async Task<TestProject?> GetActiveProjectAsync()
    {
        var principal = await GetUserClaimsPrincipalAsync();
        var preferences = await _userPreferencesService.LoadUserPreferencesAsync(principal);
        if (preferences?.ActiveProjectId is not null)
        {
            var tenantId = await GetTenantIdAsync();
            var project = await _projectRepository.GetProjectByIdAsync(tenantId, preferences.ActiveProjectId.Value);
            return project;
        }
        return null;
    }

    public async Task<TestProject?> GetProjectBySlugAsync(string slug)
    {
        var tenantId = await GetTenantIdAsync();
        return await _projectRepository.GetBySlugAsync(tenantId, slug);
    }
    public async Task<TestProject?> GetProjectByIdAsync(long id)
    {
        var tenantId = await GetTenantIdAsync();
        return await _projectRepository.GetProjectByIdAsync(tenantId, projectId: id);
    }

    public async Task SaveProjectAsync(TestProject project)
    {
        var tenantId = await GetTenantIdAsync();
        if(tenantId != project.TenantId)
        {
            throw new InvalidOperationException("Tenant ID mismatch");
        }

        await _projectRepository.UpdateProjectAsync(project);
    }

    /// <summary>
    /// Generates a short name
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="slug"></param>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    public async Task<string> GenerateShortNameAsync(string slug)
    {
        var tenantId = await GetTenantIdAsync();
        return await _projectRepository.GenerateShortNameAsync(slug, tenantId);
    }

    /// <summary>
    /// Generates a slug for a name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public string GenerateSlug(string name)
    {
        return _projectRepository.GenerateSlug(name);
    }

    public async Task<PagedResult<TestProject>> SearchAsync(SearchQuery query)
    {
        var tenantId = await GetTenantIdAsync();
        return await _projectRepository.SearchAsync(tenantId, query);
    }
    public async Task<OneOf<TestProject, AlreadyExistsError>> CreateAsync(long? teamId, string name)
    {
        var tenantId = await GetTenantIdAsync();
        return await _projectRepository.CreateAsync(teamId, tenantId, name);   
    }
}
