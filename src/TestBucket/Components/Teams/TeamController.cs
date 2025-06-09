using Microsoft.Extensions.Localization;

using OneOf;

using TestBucket.Components.Account;
using TestBucket.Components.Teams.Dialogs;
using TestBucket.Domain.Errors;
using TestBucket.Domain.Teams;
using TestBucket.Localization;

namespace TestBucket.Components.Teams;

internal class TeamController : TenantBaseService
{
    private readonly ITeamRepository _teamRepository;
    private readonly ITeamManager _teamManager;
    private readonly IDialogService _dialogService;
    private readonly UserPreferencesService _userPreferencesService;
    private readonly AppNavigationManager _appNavigationManager;
    private readonly IStringLocalizer<SharedStrings> _loc;

    public TeamController(
        ITeamRepository teamRepository,
        UserPreferencesService userPreferencesService,
        AuthenticationStateProvider authenticationStateProvider,
        ITeamManager teamManager,
        AppNavigationManager appNavigationManager,
        IDialogService dialogService,
        IStringLocalizer<SharedStrings> loc) : base(authenticationStateProvider)
    {
        _teamRepository = teamRepository;
        _userPreferencesService = userPreferencesService;
        _teamManager = teamManager;
        _appNavigationManager = appNavigationManager;
        _dialogService = dialogService;
        _loc = loc;
    }

    public async Task DeleteAsync(Team team)
    {
        var hasPermission = await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Team, PermissionLevel.Delete);
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

        // todo: messagebox 
        var principal = await GetUserClaimsPrincipalAsync();
        await _teamManager.DeleteAsync(principal, team);
    }

    public async Task SetActiveTeamAsync(Team? team)
    {
        _appNavigationManager.State.SelectedTeam = team;

        var preferences = await _userPreferencesService.GetUserPreferencesAsync();
        if(preferences is not null)
        {
            preferences.ActiveTeamId = team?.Id;
            preferences.ActiveProjectId = null;
            await _userPreferencesService.SaveUserPreferencesAsync(preferences);    
        }
    }

    public async Task<Team?> GetActiveTeamAsync()
    {
        var preferences = await _userPreferencesService.GetUserPreferencesAsync();
        if (preferences?.ActiveTeamId is not null)
        {
            var tenantId = await GetTenantIdAsync();
            var team = await _teamRepository.GetTeamByIdAsync(tenantId, preferences.ActiveTeamId.Value);
            return team;
        }
        return null;
    }

    public async Task<Team?> GetTeamBySlugAsync(string slug)
    {
        var hasPermission = await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Team, PermissionLevel.Read);
        if (!hasPermission)
            return null;

        var tenantId = await GetTenantIdAsync();
        return await _teamRepository.GetBySlugAsync(tenantId, slug);
    }
    public async Task<Team?> GetTeamByIdAsync(long id)
    {
        var hasPermission = await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Team, PermissionLevel.Read);
        if (!hasPermission)
            return null;

        var tenantId = await GetTenantIdAsync();
        return await _teamRepository.GetTeamByIdAsync(tenantId, id);
    }

    public async Task SaveTeamAsync(Team team)
    {
        var hasPermission = await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Team, PermissionLevel.Write);
        if (!hasPermission)
            return;

        var tenantId = await GetTenantIdAsync();
        if(tenantId != team.TenantId)
        {
            throw new InvalidOperationException("Tenant ID mismatch");
        }

        await _teamRepository.UpdateTeamAsync(team);
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
        return await _teamRepository.GenerateShortNameAsync(slug, tenantId);
    }

    /// <summary>
    /// Generates a slug for a name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public string GenerateSlug(string name)
    {
        return _teamRepository.GenerateSlug(name);
    }

    public async Task<Team?> AddTeamAsync()
    {
        var hasPermission = await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Team, PermissionLevel.Write);
        if (!hasPermission)
            return null;

        var dialog = await _dialogService.ShowAsync<AddTeamDialog>();
        var result = await dialog.Result;
        if(result?.Data is Team team)
        {
            await SetActiveTeamAsync(team);
            return team;
        }
        return null;
    }

    public async Task<PagedResult<Team>> SearchAsync(SearchQuery query)
    {
        var hasPermission = await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Team, PermissionLevel.Read);
        if (!hasPermission)
            return new PagedResult<Team>() { Items = [], TotalCount = 0 };

        var tenantId = await GetTenantIdAsync();
        return await _teamRepository.SearchAsync(tenantId, query);
    }
    public async Task<OneOf<Team, AlreadyExistsError>> CreateAsync(string name)
    {
        var hasPermission = await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.Team, PermissionLevel.Write);
        if (!hasPermission)
            throw new UnauthorizedAccessException();

        var tenantId = await GetTenantIdAsync();
        return await _teamRepository.CreateAsync(tenantId, name);   
    }
}
