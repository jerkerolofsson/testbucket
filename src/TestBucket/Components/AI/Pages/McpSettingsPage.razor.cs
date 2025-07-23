using TestBucket.Domain.AI.Mcp.Models;

namespace TestBucket.Components.AI.Pages;
public partial class McpSettingsPage
{
    [Parameter] public string TenantId { get; set; } = "";

    /// <summary>
    /// Currently selected project
    /// </summary>
    [CascadingParameter] public TestProject? Project { get; set; }

    private ApplicationUserApiKey? _key;
    private TestProject? _project;
    private List<McpServerRegistration> _serverRegistrations = [];

    private string GetMcpServerRegistrationName(McpServerRegistration mcpServer)
    {
        if(mcpServer.Configuration.Servers?.Count > 0)
        {
            return mcpServer.Configuration.Servers.FirstOrDefault().Key;
        }

        return "MCP Server";
    }

    public async Task AddApiKeyAsync()
    {
        _key = await controller.AddApiKeyAsync(Project?.Id, "MCP Access Token");
    }


    private async Task DeleteMcpServerAsync(McpServerRegistration mcpRegistration)
    {
        if (mcpRegistration is not null)
        {
            _serverRegistrations.Remove(mcpRegistration);
            await mcpController.DeleteMcpServerAsync(mcpRegistration);
        }
    }
    private async Task OnLockedChangedAsync(McpServerRegistration mcpRegistration, bool locked)
    {
        if (mcpRegistration is not null)
        {
            mcpRegistration.Locked = locked;
            await mcpController.UpdateMcpServerAsync(mcpRegistration);
        }

    }
    private async Task OnEnabledChangedAsync(McpServerRegistration mcpRegistration, bool enabled)
    {
        if (mcpRegistration is not null)
        {
            mcpRegistration.Enabled = enabled;
            await mcpController.UpdateMcpServerAsync(mcpRegistration);
        }

    }
    private async Task OnPublicForProjectChangedAsync(McpServerRegistration mcpRegistration, bool publicForProject)
    {
        if (mcpRegistration is not null)
        {
            mcpRegistration.PublicForProject = publicForProject;
            await mcpController.UpdateMcpServerAsync(mcpRegistration);
        }

    }

    private async Task AddMcpServerAsync()
    {
        if (Project is not null)
        {
            var mcpRegistration = await mcpController.AddMcpServerAsync(Project);
            if (mcpRegistration is not null)
            {
                _serverRegistrations.Add(mcpRegistration);
            }
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if(Project is not null && _project?.Id != Project.Id)
        {
            _project = Project;

            _serverRegistrations = (await mcpController.GetMcpServerRegistrationsAsync()).ToList();
            this.StateHasChanged();
        }
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