using OneOf;

using TestBucket.Components.Shared;
using TestBucket.Contracts.Integrations;
using TestBucket.Contracts.Testing.Models;
using TestBucket.Domain.Errors;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Projects;
using TestBucket.Domain.States;
using TestBucket.Domain.Tenants.Models;

namespace TestBucket.Components.Projects;

internal class ProjectController : TenantBaseService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserPreferencesManager _userPreferencesService;
    private readonly AppNavigationManager _appNavigationManager;
    private readonly IStateService _stateService;
    private readonly IProjectManager _projectManager;
    private readonly IPipelineProjectManager _pipelineManager;

    public ProjectController(
        IProjectRepository projectRepository,
        IUserPreferencesManager userPreferencesService,
        AppNavigationManager appNavigationManager,
        AuthenticationStateProvider authenticationStateProvider,
        IStateService stateService,
        IProjectManager projectManager,
        IPipelineProjectManager pipelineManager) : base(authenticationStateProvider)
    {
        _projectRepository = projectRepository;
        _userPreferencesService = userPreferencesService;
        _appNavigationManager = appNavigationManager;
        _stateService = stateService;
        _projectManager = projectManager;
        _pipelineManager = pipelineManager;
    }

    public async Task SetActiveProjectAsync(TestProject? project)
    {
        _appNavigationManager.State.SelectedProject = project;

        var principal = await GetUserClaimsPrincipalAsync();
        var preferences = await _userPreferencesService.LoadUserPreferencesAsync(principal);
        if(preferences is not null)
        {
            preferences.ActiveProjectId = project?.Id;
            await _userPreferencesService.SaveUserPreferencesAsync(principal, preferences);    
        }
    }

    /// <summary>
    /// Returns available pipeline runners for the specified project
    /// </summary>
    /// <param name="testProjectId"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<IExternalPipelineRunner>> GetPipelineRunnersAsync(long testProjectId)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _pipelineManager.GetExternalPipelineRunnersAsync(principal, testProjectId);
    }
    public async Task<TestState[]> GetStatesAsync(long? projectId)
    {
        if(projectId is null)
        {
            return _stateService.GetDefaultStates();
        }
        var principal = await GetUserClaimsPrincipalAsync();
        return await _stateService.GetProjectStatesAsync(principal, projectId.Value);
    }

    public async Task<TestProject?> GetActiveProjectAsync()
    {
        var principal = await GetUserClaimsPrincipalAsync();
        var preferences = await _userPreferencesService.LoadUserPreferencesAsync(principal);
        if (preferences?.ActiveProjectId is not null)
        {
            var tenantId = await GetTenantIdAsync();
            var project = await _projectRepository.GetProjectByIdAsync(tenantId, preferences.ActiveProjectId.Value);

            if(project is not null)
            {
                _appNavigationManager.State.SelectedProject = project;
            }

            return project;
        }
        return null;
    }

    public async Task SaveProjectIntegrationsAsync(string slug, ExternalSystem system)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _projectManager.SaveProjectIntegrationsAsync(principal, slug, system);
    }

    public async Task<IReadOnlyList<ExternalSystem>> GetProjectIntegrationsAsync(string slug)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _projectManager.GetProjectIntegrationsAsync(principal, slug);
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
