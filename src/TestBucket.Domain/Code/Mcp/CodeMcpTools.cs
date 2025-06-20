using System.ComponentModel;

using ModelContextProtocol.Server;

using TestBucket.Domain.AI.Mcp;
using TestBucket.Domain.ApiKeys;
using TestBucket.Domain.Code.Mapping;
using TestBucket.Domain.Code.Services;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Code.Mcp;

[McpServerToolType]
public class CodeMcpTools : AuthenticatedTool
{
    private readonly IArchitectureManager _architectureManager;

    public CodeMcpTools(IArchitectureManager architecture, IApiKeyAuthenticator apiKeyAuthenticator) : base(apiKeyAuthenticator)
    {
        _architectureManager = architecture;
    }

    [McpServerTool(Name = "search-features", Title ="Search features"), Description("Searches for features")]
    public async Task<IReadOnlyList<AritecturalComponentDto>> SearchFeaturesAsync(string searchText, int offset = 0, int count = 1)
    {
        if(count == 0)
        {
            count = 1;
        }

        var isAuthenticated = await IsAuthenticatedAsync();
        if (isAuthenticated && _principal is not null)
        {
            var projectId = _principal.GetProjectId();
            if (projectId is not null)
            {
                var result = await _architectureManager.SearchFeaturesAsync(_principal, projectId.Value, searchText, offset, count);
                return result.Select(x => AritecturalComponentMapper.ToDto(x)).ToList();
            }
        }
        return [];
    }
}
