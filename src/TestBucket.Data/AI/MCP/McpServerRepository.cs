using TestBucket.Domain.AI.Mcp;
using TestBucket.Domain.AI.Mcp.Models;

namespace TestBucket.Data.AI.MCP;
internal class McpServerRepository : IMcpServerRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public McpServerRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<IReadOnlyList<McpServerRegistration>> GetMcpServersForUserAsync(string tenantId, long projectId, string userName)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.McpServerRegistrations
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.TestProjectId == projectId && (x.PublicForProject || x.CreatedBy == userName))
            .ToListAsync();
    }
    public async Task<IReadOnlyList<McpServerRegistration>> GetAllAsync(string tenantId, long projectId)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.McpServerRegistrations
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.TestProjectId == projectId)
            .ToListAsync();
    }

    public async Task AddAsync(McpServerRegistration registration)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.McpServerRegistrations.Add(registration);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(McpServerRegistration registration)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.McpServerRegistrations.Update(registration);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(McpServerRegistration registration)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.McpServerRegistrations.Remove(registration);
        await dbContext.SaveChangesAsync();
    }
}
