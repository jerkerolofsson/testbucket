using TestBucket.Domain.AI.Mcp;
using TestBucket.Domain.AI.Mcp.Models;

namespace TestBucket.Data.AI.MCP;
internal class McpUserInputRepository : IMcpUserInputRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public McpUserInputRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<McpServerUserInput?> GetUserInputAsync(long projectId, string userName, long mcpServerRegistrationId, string id)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.McpServerUserInputs
            .AsNoTracking()
            .Where(x => x.TestProjectId == projectId && 
                       x.UserName == userName && 
                       x.McpServerRegistrationId == mcpServerRegistrationId &&
                       x.InputId == id)
            .FirstOrDefaultAsync();
    }

    public async Task<McpServerUserInput?> ClearUserInputsAsync(long projectId, string userName, long mcpServerRegistrationId)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        
        var userInputsToDelete = await dbContext.McpServerUserInputs
            .Where(x => x.TestProjectId == projectId && 
                       x.UserName == userName && 
                       x.McpServerRegistrationId == mcpServerRegistrationId)
            .ToListAsync();

        if (userInputsToDelete.Any())
        {
            dbContext.McpServerUserInputs.RemoveRange(userInputsToDelete);
            await dbContext.SaveChangesAsync();
            return userInputsToDelete.FirstOrDefault(); // Return first deleted item as per interface contract
        }

        return null;
    }

    public async Task AddAsync(McpServerUserInput userInput)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.McpServerUserInputs.Add(userInput);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(McpServerUserInput userInput)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.McpServerUserInputs.Update(userInput);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(McpServerUserInput userInput)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.McpServerUserInputs.Remove(userInput);
        await dbContext.SaveChangesAsync();
    }
}
