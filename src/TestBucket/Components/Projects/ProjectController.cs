using MudBlazor;

using OneOf;

using TestBucket.Components.Projects.Dialogs;
using TestBucket.Contracts.Integrations;
using TestBucket.Contracts.Requirements.States;
using TestBucket.Contracts.Testing.States;
using TestBucket.Domain.Automation.Pipelines;
using TestBucket.Domain.Errors;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Shared;
using TestBucket.Domain.States;
using TestBucket.Domain.Teams;
using TestBucket.Domain.Teams.Models;

namespace TestBucket.Components.Projects;

internal class ProjectController : TenantBaseService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserPreferencesManager _userPreferencesService;
    private readonly AppNavigationManager _appNavigationManager;
    private readonly IStateService _stateService;
    private readonly IProjectManager _projectManager;
    private readonly IPipelineProjectManager _pipelineProjectManager;
    private readonly IPipelineManager _pipelineManager;
    private readonly IDialogService _dialogService;

    public ProjectController(
        IProjectRepository projectRepository,
        IUserPreferencesManager userPreferencesService,
        AppNavigationManager appNavigationManager,
        AuthenticationStateProvider authenticationStateProvider,
        IStateService stateService,
        IProjectManager projectManager,
        IPipelineProjectManager pipelineProjectManager,
        IPipelineManager pipelineManager,
        IDialogService dialogService) : base(authenticationStateProvider)
    {
        _projectRepository = projectRepository;
        _userPreferencesService = userPreferencesService;
        _appNavigationManager = appNavigationManager;
        _stateService = stateService;
        _projectManager = projectManager;
        _pipelineProjectManager = pipelineProjectManager;
        _pipelineManager = pipelineManager;
        _dialogService = dialogService;
    }

    public async Task EditProjectIntegrationAsync(TestProject project, ExternalSystem system, IExtension extension)
    {
        var principal = await GetUserClaimsPrincipalAsync();

        var parameters = new DialogParameters<EditIntegrationDialog>
        {
            { x => x.ExternalSystem, system },
            { x => x.Extension, extension }
        };
        var dialog = await _dialogService.ShowAsync<EditIntegrationDialog>(null, parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        if (result?.Data is ExternalSystem integration)
        {
            integration.TestProjectId = project.Id;
            await _projectManager.SaveProjectIntegrationAsync(principal, project.Slug, integration);
        }
    }
    public async Task AddProjectIntegrationAsync(TestProject project, IExtension extension)
    {
        var principal = await GetUserClaimsPrincipalAsync();

        var system = new ExternalSystem 
        { 
            Name  = extension.FriendlyName,
            Provider = extension.SystemName,
            SupportedCapabilities = extension.SupportedCapabilities,
        };

        var parameters = new DialogParameters<EditIntegrationDialog>
        {
            { x => x.ExternalSystem, system },
            { x => x.Extension, extension }
        };
        var dialog = await _dialogService.ShowAsync<EditIntegrationDialog>(null, parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        if(result?.Data is ExternalSystem integration)
        {
            integration.TestProjectId = project.Id;
            await _projectManager.SaveProjectIntegrationAsync(principal, project.Slug, integration);
        }
    }
    public async Task AddProjectAsync(Team team)
    {
        var parameters = new DialogParameters<AddProjectDialog>
        {
            { x=>x.Team, team }
        };
        var dialog = await _dialogService.ShowAsync<AddProjectDialog>(null, parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
    }

    public async Task SetActiveProjectAsync(TestProject? project)
    {
        _appNavigationManager.State.SelectedProject = project;

        var principal = await GetUserClaimsPrincipalAsync();
        var preferences = await _userPreferencesService.LoadUserPreferencesAsync(principal);
        if(preferences is not null)
        {
            preferences.ActiveTeamId = project?.TeamId;
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

    /// <summary>
    /// Returns possible states for test case runs
    /// </summary>
    /// <param name="projectId"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<TestState>> GetTestCaseRunStatesAsync(long projectId)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _stateService.GetTestCaseRunStatesAsync(principal, projectId);
    }

    /// <summary>
    /// Returns possible states for test case runs
    /// </summary>
    /// <param name="projectId"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<RequirementState>> GetRequirementStatesAsync(long projectId)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _stateService.GetRequirementStatesAsync(principal, projectId);
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

    public async Task DeleteAsync(TestProject project)
    {
        // todo: messagebox 
        var principal = await GetUserClaimsPrincipalAsync();
        await _projectManager.DeleteAsync(principal, project);
    }

    public async Task DeleteProjectIntegrationAsync(ExternalSystem system)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _projectManager.DeleteProjectIntegrationAsync(principal, system.Id);
    }

    public async Task SaveProjectIntegrationAsync(string slug, ExternalSystem system)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _projectManager.SaveProjectIntegrationAsync(principal, slug, system);
    }

    public async Task<IReadOnlyList<ExternalSystem>> GetProjectIntegrationsAsync(long projectId)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _projectManager.GetProjectIntegrationsAsync(principal, projectId);
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
        var principal = await GetUserClaimsPrincipalAsync();
        principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Write);
        principal.ThrowIfEntityTenantIsDifferent(project.TenantId);

        await _projectManager.UpdateProjectAsync(principal, project);
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
    public async Task<OneOf<TestProject, AlreadyExistsError>> CreateAsync(long teamId, string name)
    {
        var principal = await GetUserClaimsPrincipalAsync();

        var slug = GenerateSlug(name);
        var shortName = await GenerateShortNameAsync(slug);
        var project= new TestProject { Name = name, Slug = slug, ShortName = shortName, TeamId = teamId };
        return await _projectManager.AddAsync(principal, project);
    }
}
