using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using Microsoft.Extensions.Logging;

using TestBucket.Contracts.Integrations;
using TestBucket.Domain.AI.Mcp.Models;
using TestBucket.Domain.Projects.Events;
using TestBucket.Domain.Projects.Mapping;
using TestBucket.Integrations;

namespace TestBucket.Domain.AI.Mcp.Integrations;

/// <summary>
/// Adds/removes an MCP server if extension supports 
/// </summary>
internal class AddOrRemoveMcpServerWhenProjectIntegrationIsUpdated : INotificationHandler<ProjectIntegrationUpdated>
{
    private readonly IMcpServerManager _mcpServerManager;
    private readonly IReadOnlyList<IExternalMcpProvider> _providers;
    private readonly ILogger<AddOrRemoveMcpServerWhenProjectIntegrationIsUpdated> _logger;

    public AddOrRemoveMcpServerWhenProjectIntegrationIsUpdated(IMcpServerManager mcpServerManager, IEnumerable<IExternalMcpProvider> providers, ILogger<AddOrRemoveMcpServerWhenProjectIntegrationIsUpdated> logger)
    {
        _mcpServerManager = mcpServerManager;
        _providers = providers.ToList();
        _logger = logger;
    }

    public async ValueTask Handle(ProjectIntegrationUpdated notification, CancellationToken cancellationToken)
    {
        try
        {
            var system = notification.ExternalSystem;

            if (system.TestProjectId is null)
            {
                return;
            }

            var projectId = system.TestProjectId.Value;
            var owner = "int:" + system.Name + " " + system.ExternalProjectId + projectId;

            var principal = notification.Principal;
            var registrations = await _mcpServerManager.GetAllMcpServerRegistationsAsync(principal, projectId);
            var integrationRegistrations = registrations.Where(x => x.Owner == owner).ToList();

            foreach (var registration in integrationRegistrations)
            {
                await _mcpServerManager.DeleteMcpServerRegistrationAsync(principal, registration);
            }

            if ((system.EnabledCapabilities & ExternalSystemCapability.McpServerProvider) == ExternalSystemCapability.McpServerProvider)
            {
                var provider = _providers.Where(x => x.SystemName == system.Provider).FirstOrDefault();
                if (provider is not null)
                {
                    var json = await provider.GetMcpJsonConfigurationAsync(system.ToDto());
                    if (json is not null)
                    {
                        var configuration = McpServerConfiguration.FromJson(json);
                        if (configuration is not null)
                        {
                            var registration = new McpServerRegistration { Configuration = configuration, Owner = owner, Enabled = true, PublicForProject = true, TestProjectId = projectId };
                            await _mcpServerManager.AddMcpServerRegistrationAsync(principal, registration);
                        }
                    }
                }
            }
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error updating MCP server registrations when integration/extension is changed");
        }
    }
}
