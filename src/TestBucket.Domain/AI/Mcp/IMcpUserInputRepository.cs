using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.AI.Mcp.Models;

namespace TestBucket.Domain.AI.Mcp;
public interface IMcpUserInputRepository
{
    Task<McpServerUserInput?> GetUserInputAsync(long projectId, string userName, long mcpServerRegistrationId, string id);

    /// <summary>
    /// Deletes all mcp user inputs for a specific user and MCP server registration.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="userName"></param>
    /// <param name="mcpServerRegistrationId"></param>
    /// <returns></returns>
    Task<McpServerUserInput?> ClearUserInputsAsync(long projectId, string userName, long mcpServerRegistrationId);
    Task AddAsync(McpServerUserInput userInput);
    Task UpdateAsync(McpServerUserInput userInput);
    Task DeleteAsync(McpServerUserInput userInput);
}
