using Microsoft.Extensions.Localization;

using OneOf;

using TestBucket.Components.Projects.Dialogs;
using TestBucket.Contracts.Integrations;
using TestBucket.Contracts.Issues.States;
using TestBucket.Contracts.Issues.Types;
using TestBucket.Contracts.Requirements.States;
using TestBucket.Contracts.Requirements.Types;
using TestBucket.Contracts.Testing.States;
using TestBucket.Domain.Automation.Pipelines;
using TestBucket.Domain.Errors;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Shared;
using TestBucket.Domain.States;
using TestBucket.Localization;
using TestBucket.Domain.Projects.Mapping;

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
    private readonly IStringLocalizer<SharedStrings> _loc;
    public ProjectController(
        IProjectRepository projectRepository,
        IUserPreferencesManager userPreferencesService,
        AppNavigationManager appNavigationManager,
        AuthenticationStateProvider authenticationStateProvider,
        IStateService stateService,
        IProjectManager projectManager,
        IPipelineProjectManager pipelineProjectManager,
        IPipelineManager pipelineManager,
        IDialogService dialogService,
        IStringLocalizer<SharedStrings> loc) : base(authenticationStateProvider)
    {
        _projectRepository = projectRepository;
        _userPreferencesService = userPreferencesService;
        _appNavigationManager = appNavigationManager;
        _stateService = stateService;
        _projectManager = projectManager;
        _pipelineProjectManager = pipelineProjectManager;
        _pipelineManager = pipelineManager;
        _dialogService = dialogService;
        _loc = loc;
    }

    public async Task EditProjectIntegrationAsync(TestProject project, ExternalSystem system, IExtension extension)
    {
        var hasPermission = await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Project, PermissionLevel.Write);
        if (!hasPermission)
            return;

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
        var hasPermission = await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Project, PermissionLevel.Write);
        if (!hasPermission)
            return;

        var principal = await GetUserClaimsPrincipalAsync();

        var systemDto = new ExternalSystemDto
        { 
            Id = 0,
            Enabled = true,
            Name  = extension.FriendlyName,
            Provider = extension.SystemName,
            SupportedCapabilities = extension.SupportedCapabilities,
            EnabledCapabilities = extension.SupportedCapabilities,
            TestResultsArtifactsPattern = "**/*xunit*.xml;**/*junit*.xml;**/*nunit*.xml;**/*test*.xml;**/*.trx;**/*.ctrf",
            CoverageReportArtifactsPattern = "**/*coverage*.xml",
            BaseUrl = extension.DefaultBaseUrl
        };

        extension.ConfigureDefaults(systemDto);
        var system = systemDto.ToDbo();

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
        var hasPermission = await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Project, PermissionLevel.Write);
        if (!hasPermission)
            return;


        var parameters = new DialogParameters<AddProjectDialog>
        {
            { x=>x.Team, team }
        };
        var dialog = await _dialogService.ShowAsync<AddProjectDialog>(null, parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        if (result?.Data is TestProject project)
        {
            await SetActiveProjectAsync(project);
        }
    }

    public async Task SetActiveProjectAsync(TestProject? project)
    {
        _appNavigationManager.State.SelectedProject = project;
        _appNavigationManager.State.SelectedTeam = project?.Team;

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
        var hasPermission = await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Project, PermissionLevel.Read);
        if (!hasPermission)
            return []; 

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
        var hasPermission = await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Project, PermissionLevel.Read);
        if (!hasPermission)
            return [];

        var principal = await GetUserClaimsPrincipalAsync();
        return await _stateService.GetTestCaseRunStatesAsync(principal, projectId);
    }

    /// <summary>
    /// Returns possible states for issues
    /// </summary>
    /// <param name="projectId"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<IssueState>> GetIssueStatesAsync(long projectId)
    {
        var hasPermission = await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Project, PermissionLevel.Read);
        if (!hasPermission)
            return [];

        var principal = await GetUserClaimsPrincipalAsync();
        return await _stateService.GetIssueStatesAsync(principal, projectId);
    }

    /// <summary>
    /// Returns possible states for requirements
    /// </summary>
    /// <param name="projectId"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<RequirementState>> GetRequirementStatesAsync(long projectId)
    {
        var hasPermission = await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Project, PermissionLevel.Read);
        if (!hasPermission)
            return [];

        var principal = await GetUserClaimsPrincipalAsync();
        return await _stateService.GetRequirementStatesAsync(principal, projectId);
    }

    /// <summary>
    /// Returns possible types of requirements
    /// </summary>
    /// <param name="projectId"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<RequirementType>> GetRequirementTypesAsync(long projectId)
    {
        var hasPermission = await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Project, PermissionLevel.Read);
        if (!hasPermission)
            return [];

        var principal = await GetUserClaimsPrincipalAsync();
        return await _stateService.GetRequirementTypesAsync(principal, projectId);
    }

    /// <summary>
    /// Returns possible types of issues
    /// </summary>
    /// <param name="projectId"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<IssueType>> GetIssueTypesAsync(long projectId)
    {
        var hasPermission = await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Project, PermissionLevel.Read);
        if (!hasPermission)
            return [];

        var principal = await GetUserClaimsPrincipalAsync();
        return await _stateService.GetIssueTypesAsync(principal, projectId);
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
                _appNavigationManager.State.SelectedTeam = project?.Team;
            }

            return project;
        }
        return null;
    }

    public async Task DeleteAsync(TestProject project)
    {
        var hasPermission = await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Project, PermissionLevel.Delete);
        if (!hasPermission)
            return;

        // Show confirmation dialog before deleting
        var confirmResult = await _dialogService.ShowMessageBox(new MessageBoxOptions
        {
            YesText = _loc["yes"],
            NoText = _loc["no"],
            Title = _loc["confirm-delete-title"],
            MarkupMessage = new MarkupString(_loc["confirm-delete-message"])
        });
        if (confirmResult is false)
            return;

        var principal = await GetUserClaimsPrincipalAsync();
        await _projectManager.DeleteAsync(principal, project);
    }

    public async Task DeleteProjectIntegrationAsync(ExternalSystem system)
    {
        var hasPermission = await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Project, PermissionLevel.Write);
        if (!hasPermission)
            return;

        var principal = await GetUserClaimsPrincipalAsync();
        await _projectManager.DeleteProjectIntegrationAsync(principal, system.Id);
    }

    public async Task SaveProjectIntegrationAsync(string slug, ExternalSystem system)
    {
        var hasPermission = await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Project, PermissionLevel.Write);
        if (!hasPermission)
            return;

        var principal = await GetUserClaimsPrincipalAsync();
        await _projectManager.SaveProjectIntegrationAsync(principal, slug, system);
    }

    public async Task<IReadOnlyList<ExternalSystem>> GetProjectIntegrationsAsync(long projectId)
    {
        var hasPermission = await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Project, PermissionLevel.Read);
        if (!hasPermission)
            return [];

        var principal = await GetUserClaimsPrincipalAsync();
        return await _projectManager.GetProjectIntegrationsAsync(principal, projectId);
    }
    public async Task<IReadOnlyList<ExternalSystem>> GetProjectIntegrationsAsync(string slug)
    {
        var hasPermission = await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Project, PermissionLevel.Read);
        if (!hasPermission)
            return [];

        var principal = await GetUserClaimsPrincipalAsync();
        return await _projectManager.GetProjectIntegrationsAsync(principal, slug);
    }

    public async Task<TestProject?> GetProjectBySlugAsync(string slug)
    {
        var hasPermission = await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Project, PermissionLevel.Read);
        if (!hasPermission)
            return null;

        var tenantId = await GetTenantIdAsync();
        return await _projectRepository.GetBySlugAsync(tenantId, slug);
    }
    public async Task<TestProject?> GetProjectByIdAsync(long id)
    {
        var hasPermission = await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Project, PermissionLevel.Read);
        if (!hasPermission)
            return null;

        var tenantId = await GetTenantIdAsync();
        return await _projectRepository.GetProjectByIdAsync(tenantId, projectId: id);
    }

    public async Task SaveProjectAsync(TestProject project)
    {
        var hasPermission = await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Project, PermissionLevel.Write);
        if (!hasPermission)
            return;

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
        var hasPermission = await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Project, PermissionLevel.Read);
        if (!hasPermission)
            return new PagedResult<TestProject> { Items = [], TotalCount = 0 };

        var tenantId = await GetTenantIdAsync();
        return await _projectRepository.SearchAsync(tenantId, query);
    }
    public async Task<OneOf<TestProject, AlreadyExistsError>> CreateAsync(long teamId, string name)
    {
        var hasPermission = await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Project, PermissionLevel.Write);
        if (!hasPermission)
            throw new UnauthorizedAccessException();

        var principal = await GetUserClaimsPrincipalAsync();

        var slug = GenerateSlug(name);
        var shortName = await GenerateShortNameAsync(slug);
        var project= new TestProject { Name = name, Slug = slug, ShortName = shortName, TeamId = teamId };
        return await _projectManager.AddAsync(principal, project);
    }
}
