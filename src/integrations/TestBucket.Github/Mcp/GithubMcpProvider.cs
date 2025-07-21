using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Integrations;
using TestBucket.Integrations;

namespace TestBucket.Github.Mcp;
internal class GithubMcpProvider : IExternalMcpProvider
{
    public string SystemName => ExtensionConstants.SystemName;

    public Task<string?> GetMcpJsonConfigurationAsync(ExternalSystemDto system)
    {
        string? configuration = null;
        if((system.EnabledCapabilities&ExternalSystemCapability.McpServerProvider) == ExternalSystemCapability.McpServerProvider)
        {
            configuration = $$"""
                {
                  "servers": {
                    "github": {
                      "type": "http",
                      "url": "https://api.githubcopilot.com/mcp/",
                      "headers": {
                        "Authorization": "Bearer {{system.AccessToken}}"
                      }
                    }
                  }
                }
                """;
        }
        return Task.FromResult(configuration);
    }
}
