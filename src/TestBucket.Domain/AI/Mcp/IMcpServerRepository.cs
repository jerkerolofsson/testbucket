using TestBucket.Domain.AI.Mcp.Models;

namespace TestBucket.Domain.AI.Mcp;
public interface IMcpServerRepository
{
    Task<IReadOnlyList<McpServerRegistration>> GetAllAsync(string tenantId);
    Task<IReadOnlyList<McpServerRegistration>> GetAllAsync(string tenantId, long projectId);
    Task AddAsync(McpServerRegistration registration);
    Task UpdateAsync(McpServerRegistration registration);
    Task DeleteAsync(McpServerRegistration registration);
    Task<IReadOnlyList<McpServerRegistration>> GetMcpServersForUserAsync(string v, long projectId, string userName);
}
