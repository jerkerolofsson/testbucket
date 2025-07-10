using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ModelContextProtocol.Server;

using TestBucket.Domain.AI.Mcp.Tools;
using TestBucket.Domain.ApiKeys;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Heuristics.Models;

namespace TestBucket.Domain.Testing.Heuristics.Mcp;

[McpServerToolType]

public class HeuristicsMcpTools : AuthenticatedTool
{
    private readonly IHeuristicsManager _manager;

    public HeuristicsMcpTools(IHeuristicsManager manager, IApiKeyAuthenticator authenticator) : base(authenticator)
    {
        _manager = manager;
    }

    /// <summary>
    /// Returns the name of the user that is authenticated with the API key.
    /// </summary>
    /// <returns></returns>
    [McpServerTool(Name = "get-hueristics"), Description("Gets a list of test heuristics, containing general ideas and best-practices for how to do exploratory testing and ideas to consider when creating new test cases")]
    public async Task<string> GetHeuristics()
    {
        var isAuthenticated = await IsAuthenticatedAsync();
        if (isAuthenticated && _principal is not null)
        {
            var projectId = _principal.GetProjectId();
            if (projectId is null)
            {
                throw new ArgumentException("The user was authenticated but the project was defined in the claims");
            }
            FilterSpecification<Heuristic>[] filters = [new FilterByProject<Heuristic>(projectId.Value)];
            var heuristics = await _manager.SearchAsync(_principal, filters, 0, 100);

            var sb = new StringBuilder();

            foreach(var item in heuristics.Items)
            {
                sb.AppendLine($"# {item.Name}");
                sb.AppendLine($"{item.Description}");
            }

            return sb.ToString();
        }
        return "";
    }
}
