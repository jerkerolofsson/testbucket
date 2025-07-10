using TestBucket.Domain.AI.Mcp.Models;

namespace TestBucket.Domain.AI.Mcp.Services;
public interface IMcpServerUserInputProvider
{
    Task<McpServerUserInput?> GetUserInputAsync(long projectId, string userName, long mcpServerRegistrationId, string id);
}
