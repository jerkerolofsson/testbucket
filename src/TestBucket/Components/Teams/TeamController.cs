using OneOf;

using TestBucket.Components.Account;
using TestBucket.Domain.Errors;
using TestBucket.Domain.Teams;
using TestBucket.Domain.Teams.Models;

namespace TestBucket.Components.Teams;

internal class TeamController : TenantBaseService
{
    private readonly ITeamRepository _teamRepository;
    private readonly UserPreferencesService _userPreferencesService;

    public TeamController(
        ITeamRepository teamRepository,
        UserPreferencesService userPreferencesService,
        AuthenticationStateProvider authenticationStateProvider) : base(authenticationStateProvider)
    {
        _teamRepository = teamRepository;
        _userPreferencesService = userPreferencesService;
    }

    public async Task SetActiveTeamAsync(Team? team)
    {
        var preferences = await _userPreferencesService.GetUserPreferencesAsync();
        if(preferences is not null)
        {
            preferences.ActiveTeamId = team?.Id;
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
        var tenantId = await GetTenantIdAsync();
        return await _teamRepository.GetBySlugAsync(tenantId, slug);
    }
    public async Task<Team?> GetTeamByIdAsync(long id)
    {
        var tenantId = await GetTenantIdAsync();
        return await _teamRepository.GetTeamByIdAsync(tenantId, id);
    }

    public async Task SaveTeamAsync(Team team)
    {
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

    public async Task<PagedResult<Team>> SearchAsync(SearchQuery query)
    {
        var tenantId = await GetTenantIdAsync();
        return await _teamRepository.SearchAsync(tenantId, query);
    }
    public async Task<OneOf<Team, AlreadyExistsError>> CreateAsync(string name)
    {
        var tenantId = await GetTenantIdAsync();
        return await _teamRepository.CreateAsync(tenantId, name);   
    }
}
