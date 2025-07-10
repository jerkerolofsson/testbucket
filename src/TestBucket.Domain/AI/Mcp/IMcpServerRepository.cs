using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.AI.Mcp.Models;

namespace TestBucket.Domain.AI.Mcp;
public interface IMcpServerRepository
{
    Task<IReadOnlyList<McpServerRegistration>> GetAllAsync(string tenantId, long projectId);
    Task AddAsync(McpServerRegistration registration);
    Task UpdateAsync(McpServerRegistration registration);
    Task DeleteAsync(McpServerRegistration registration);
    Task<IReadOnlyList<McpServerRegistration>> GetMcpServersForUserAsync(string v, long projectId, string userName);
}
