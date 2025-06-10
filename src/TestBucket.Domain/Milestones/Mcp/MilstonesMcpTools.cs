using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ModelContextProtocol.Server;

using TestBucket.Domain.AI.Mcp;
using TestBucket.Domain.ApiKeys;
using TestBucket.Domain.Testing.TestRuns;
using TestBucket.Domain.Testing.TestRuns.Search;

namespace TestBucket.Domain.Milestones.Mcp;

[McpServerToolType]
public class MilstonesMcpTools : AuthenticatedTool
{
    private readonly IMilestoneManager _manager;

    public MilstonesMcpTools(IMilestoneManager manager, IApiKeyAuthenticator apiKeyAuthenticator) : base(apiKeyAuthenticator)
    {
        _manager = manager;
    }

    /// <summary>
    /// Returns the latest test results
    /// </summary>
    /// <returns></returns>
    [McpServerTool(Name = "list-milestones"), Description("Returns a list of all project milestones")]
    public async Task<string[]> GetMilestonesAsync()
    {
        var isAuthenticated = await IsAuthenticatedAsync();
        if (isAuthenticated && _principal is not null)
        {
            var projectId = _principal.GetProjectId();
            if (projectId is not null)
            {
                var result = await _manager.GetMilestonesAsync(_principal, projectId.Value);
                return result.Where(x=>x.Title != null).Select(x => x.Title!).ToArray();
            }

        }
        return [];
    }
}
