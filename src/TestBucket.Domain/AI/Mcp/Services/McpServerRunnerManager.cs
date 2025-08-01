using System.Collections.Concurrent;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using TestBucket.Domain.AI.Agent;
using TestBucket.Domain.AI.Mcp.Helpers;
using TestBucket.Domain.AI.Mcp.Models;
using TestBucket.Domain.AI.Mcp.Tools;
using TestBucket.Domain.Automation.Runners.Models;

namespace TestBucket.Domain.AI.Mcp.Services;

/// <summary>
/// Manages running MCP servers.
/// </summary>
public class McpServerRunnerManager
{
    private readonly ConcurrentDictionary<long, McpServerRegistration> _serverRegistrations = new();
    private readonly ConcurrentDictionary<long, List<McpServerRunner>> _runners = new();
    private readonly ILogger<McpServerRunnerManager> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public McpServerRunnerManager(ILogger<McpServerRunnerManager> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<List<McpAIFunction>> GetMcpToolsForUserAsync(ClaimsPrincipal principal, AgentChatContext context, CancellationToken cancellationToken = default)
    {
        var projectId = context.ProjectId;
        var userName = principal.Identity?.Name ?? throw new ArgumentNullException(nameof(principal.Identity.Name), "User identity is required to get MCP tools.");

        // There may be multiple servers, for example "playwright", we should only return one of
        // them.
        HashSet<string> setToolTypes = [];

        List<McpAIFunction> tools = [];
        foreach (var registration in _serverRegistrations.Values)
        {
            if (_runners.TryGetValue(registration.Id, out var runners))
            {
                foreach (var runner in runners)
                {
                    if (McpToolAccessChecker.HasAccess(projectId, principal, runner.Registration))
                    {
                        foreach (var tool in await runner.GetToolsForSessionAsync(userName, cancellationToken))
                        {
                            tool.Enabled = !setToolTypes.Contains(runner.McpToolName);
                            tool.ToolName = runner.McpToolName;
                            tools.Add(tool);
                        }

                        if (!setToolTypes.Contains(runner.McpToolName))
                        {
                            setToolTypes.Add(runner.McpToolName);
                        }
                    }
                }
            }
        }

        return tools;
    }

    public async Task StartServerAsync(McpServerRegistration registration, CancellationToken cancellationToken)
    {
        if(!registration.Enabled)
        {
            return;
        }
        if (registration.Configuration?.Servers is null)
        {
            throw new ArgumentException("MCP server configuration is not set.");
        }

        if (registration.Configuration.Servers.Count == 0)
        {
            return;
        }

        await StopServerAsync(registration, cancellationToken);

        _serverRegistrations[registration.Id] = registration;
        _runners[registration.Id] = [];

        foreach (var serverConfig in registration.Configuration.Servers)
        {
            var name = serverConfig.Key;
            var server = serverConfig.Value;
            try
            {
                _logger.LogInformation("Creating MCP server runner: {ServerName}", name);
                var runner = await McpServerRunner.CreateAsync(registration, server, name, cancellationToken);
                _runners[registration.Id].Add(runner);

                // Clear error message if successful
                server.ErrorMessage = null;
                await UpdateRegistrationAsync(registration);
            }

            catch(OperationCanceledException)
            {
                server.ErrorMessage = "Timeout connecting to MCP server";
                await UpdateRegistrationAsync(registration);
            }
            catch (Exception ex)
            {
                server.ErrorMessage = ex.Message;
                await UpdateRegistrationAsync(registration);
            }
        }
    }

    private async Task UpdateRegistrationAsync(McpServerRegistration registration)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IMcpServerRepository>();  
        await repository.UpdateAsync(registration);
    }

    public async Task RestartServerAsync(McpServerRegistration registration, CancellationToken cancellationToken)
    {
        if (registration == null)
        {
            throw new ArgumentNullException(nameof(registration), "MCP server registration cannot be null.");
        }
        if (_serverRegistrations.ContainsKey(registration.Id))
        {
            await StopServerAsync(registration, cancellationToken);
        }
        await StartServerAsync(registration, cancellationToken);    
    }

    public async Task StopServerAsync(McpServerRegistration registration, CancellationToken cancellationToken)
    {
        if (registration == null)
        {
            throw new ArgumentNullException(nameof(registration), "MCP server registration cannot be null.");
        }
        if(_runners.TryGetValue(registration.Id, out var runners))
        {
            foreach(var runner in runners)
            {
                _logger.LogInformation("Disposing MCP server runner: {ServerName}", runner.Name);
                await runner.DisposeAsync();
            }
        }
        _serverRegistrations.TryRemove(registration.Id, out _);
    }
}
