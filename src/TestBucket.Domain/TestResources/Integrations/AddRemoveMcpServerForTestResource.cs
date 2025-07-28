using Mediator;

using TestBucket.Domain.AI.Mcp;
using TestBucket.Domain.AI.Mcp.Models;
using TestBucket.Domain.AI.Mcp.Services;
using TestBucket.Domain.TestResources.Events;

namespace TestBucket.Domain.TestResources.Integrations;
internal class AddRemoveMcpServerForTestResource :
    INotificationHandler<TestResourceAdded>,
    INotificationHandler<TestResourceUpdated>,
    INotificationHandler<TestResourceRemoved>
{
    private readonly IMcpServerManager _mcpServerManager;

    public AddRemoveMcpServerForTestResource(IMcpServerManager mcpServerManager)
    {
        _mcpServerManager = mcpServerManager;
    }

    public async ValueTask Handle(TestResourceRemoved notification, CancellationToken cancellationToken)
    {
        if (notification.Resource.Types.Contains("mcp-server"))
        {
            var resource = notification.Resource;
            var principal = notification.Principal;

            var servers = await _mcpServerManager.GetAllMcpServerRegistationsAsync(principal);
            foreach(var server in servers.Where(x=>x.Owner == resource.Owner && x.Configuration.Servers != null && x.Configuration.Servers.Any(y => y.Key == resource.Name)))
            {
                await _mcpServerManager.DeleteMcpServerRegistrationAsync(principal, server);
            }
        }
    }

    public async ValueTask Handle(TestResourceUpdated notification, CancellationToken cancellationToken)
    {
        if (notification.Resource.Types.Contains("mcp-server"))
        {
            var resource = notification.Resource;
            var principal = notification.Principal;

            if (resource.Variables.TryGetValue("url", out var url))
            {
                var servers = await _mcpServerManager.GetAllMcpServerRegistationsAsync(principal);
                bool found = false;
                foreach (var server in servers.Where(x => x.Owner == resource.Owner && x.Configuration.Servers != null && x.Configuration.Servers.Any(y => y.Key == resource.Name)))
                {
                    if (server.Configuration.Servers?.Count == 1)
                    {
                        var serverConfig = server.Configuration.Servers.Values.FirstOrDefault();
                        found = true;
                        if (serverConfig is not null && serverConfig.Url != url)
                        {
                            // URL has changed, update it
                            serverConfig.Url = url;
                            await _mcpServerManager.UpdateMcpServerRegistrationAsync(principal, server);
                        }
                    }
                }

                if(!found)
                {
                    await AddMcpServerFromResourceAsync(resource, principal, url);
                }
            }
        }
    }
    public async ValueTask Handle(TestResourceAdded notification, CancellationToken cancellationToken)
    {
        if (notification.Resource.Types.Contains("mcp-server"))
        {
            var resource = notification.Resource;
            var principal = notification.Principal;

            if (resource.Variables.TryGetValue("url", out var url))
            {
                await AddMcpServerFromResourceAsync(resource, principal, url);
            }
        }
    }

    private async Task AddMcpServerFromResourceAsync(Models.TestResource resource, ClaimsPrincipal principal, string url)
    {
        var mcpServerRegistration = new McpServerRegistration
        {
            PublicForProject = true,
            Owner = resource.Owner,
            Configuration = new McpServerConfiguration
            {
                Servers = new Dictionary<string, McpServer>
                {
                    [resource.Name] = new McpServer
                    {
                        Url = url,
                        ToolName = resource.Types.FirstOrDefault()
                    }
                }
            }
        };

        await _mcpServerManager.AddMcpServerRegistrationAsync(principal, mcpServerRegistration);
    }
}
