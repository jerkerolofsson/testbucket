using TestBucket.Domain;
using TestBucket.Domain.AI.Models;
using TestBucket.Domain.Settings;
using TestBucket.Domain.Settings.Models;

namespace TestBucket.Components.Settings.Pages;
public partial class McpSettingsPage
{
    [Parameter] public string TenantId { get; set; } = "";

    /// <summary>
    /// Currently selected project
    /// </summary>
    [CascadingParameter] public TestProject? Project { get; set; }

    private ApplicationUserApiKey? _key;

    public async Task AddApiKeyAsync()
    {
        _key = await controller.AddApiKeyAsync(Project?.Id, "MCP Access Token");
    }

    private string VisualStudioMcpRemoteConfiguration
    {
        get
        {
            string baseUrl = "";

            if (Uri.TryCreate(navigationManager.BaseUri, UriKind.Absolute, out var uri))
            {
                baseUrl = uri.GetLeftPart(UriPartial.Authority);
            }

            string token = _key?.Key ?? "<ProjectAccessToken>";

            return $$"""
                  {
                  "inputs": [
                    {
                      "id": "testbucket_pat",
                      "description": "Test bucket access token",
                      "type": "promptString",
                      "password": true
                    }
                  ],
                  "servers": {
                    "testbucket": {
                      "type": "http",
                      "url": "{{baseUrl}}/mcp",
                      "headers": {
                        "Authorization": "Bearer ${input:testbucket_pat}"
                      }
                    }
                  }
                }
                
                """;
        }
    }

    private string McpRemoteConfiguration
    {
        get
        {
            string baseUrl = "";

            if (Uri.TryCreate(navigationManager.BaseUri, UriKind.Absolute, out var uri))
            {
                baseUrl = uri.GetLeftPart(UriPartial.Authority);
            }

            string token = _key?.Key?? "<ProjectAccessToken>";

            return $$"""
                  {
                  "mcpServers": {
                    "test-bucket": {
                      	"command": "npx",
                      	"args": [
                		"mcp-remote", 
                		"{{baseUrl}}/mcp",
                		"--header",
                    		"Authorization:${AUTH_HEADER}"
                	],
                  	"env": {
                    		"AUTH_HEADER": "Bearer {{token}}"
                	  }
                    }
                  }
                }
                
                """;
        }
    }
}