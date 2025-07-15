using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.AI.Mcp;
using TestBucket.Domain.AI.Mcp.Models;
using TestBucket.Domain.TestResources.Events;

namespace TestBucket.Domain.TestResources.Integrations;
internal class AddRemoveMcpServerForTestResource :
    INotificationHandler<TestResourceAdded>,
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

    public async ValueTask Handle(TestResourceAdded notification, CancellationToken cancellationToken)
    {
        if (notification.Resource.Types.Contains("mcp-server"))
        {
            var resource = notification.Resource;
            var principal = notification.Principal;

            if (resource.Variables.TryGetValue("url", out var url))
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
                                Url = url
                            }
                        }
                    }
                };

                await _mcpServerManager.AddMcpServerRegistrationAsync(principal, mcpServerRegistration);
            }
        }
    }
}
