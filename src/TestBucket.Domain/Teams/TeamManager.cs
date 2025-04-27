using TestBucket.Domain.Teams.Models;

namespace TestBucket.Domain.Teams;
internal class TeamManager : ITeamManager
{
    private readonly ITeamRepository _repo;

    public TeamManager(ITeamRepository repo)
    {
        _repo = repo;
    }

    public async Task<OneOf<Team, AlreadyExistsError>> AddAsync(ClaimsPrincipal principal, Team team)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Team, PermissionLevel.Write);
        team.TenantId = principal.GetTenantIdOrThrow();
        return await _repo.AddAsync(team);
    }

    public async Task DeleteAsync(ClaimsPrincipal principal, Team team)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Team, PermissionLevel.Delete);
        principal.ThrowIfEntityTenantIsDifferent(team.TenantId);
        await _repo.DeleteAsync(team);
    }

    public async Task<Team?> GetTeamByIdAsync(ClaimsPrincipal principal, long id)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        return await _repo.GetTeamByIdAsync(tenantId, id);
    }
    public async Task<Team?> GetTeamBySlugAsync(ClaimsPrincipal principal, string slug)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        return await _repo.GetBySlugAsync(tenantId, slug);
    }
}
