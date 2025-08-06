using Microsoft.EntityFrameworkCore;
using TestBucket.Domain.States;
using TestBucket.Domain.States.Models;

namespace TestBucket.Data.Projects;
internal class StateRepository : IStateRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public StateRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task AddStateDefinitionAsync(StateDefinition stateDefinition)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.StateDefinitions.Add(stateDefinition);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteStateDefinitionAsync(StateDefinition stateDefinition)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.StateDefinitions.Remove(stateDefinition);
        await dbContext.SaveChangesAsync();
    }

    public async Task<StateDefinition?> GetTenantStateDefinitionAsync(string tenantId)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.StateDefinitions
            .FirstOrDefaultAsync(sd => sd.TenantId == tenantId && sd.TeamId == null && sd.TestProjectId == null);
    }

    public async Task<StateDefinition?> GetTeamStateDefinitionAsync(string tenantId, long teamId)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.StateDefinitions
            .FirstOrDefaultAsync(sd => sd.TenantId == tenantId && sd.TeamId == teamId && sd.TestProjectId == null);
    }
    public async Task<StateDefinition?> GetProjectStateDefinitionAsync(string tenantId, long projectId)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.StateDefinitions
            .FirstOrDefaultAsync(sd => sd.TenantId == tenantId && sd.TestProjectId == projectId && sd.TeamId == null);
    }

    public async Task UpdateStateDefinitionAsync(StateDefinition stateDefinition)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.StateDefinitions.Update(stateDefinition);
        await dbContext.SaveChangesAsync();
    }
}
