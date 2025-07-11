using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using ModelContextProtocol.Client;

using TestBucket.Domain.AI.Mcp.Models;
using TestBucket.Domain.AI.Mcp.Tools;

namespace TestBucket.Domain.AI.Mcp.Services;

/// <summary>
/// Manages running MCP servers.
/// </summary>
public class McpServerRunnerManager
{
    private readonly ConcurrentDictionary<long, McpServerRegistration> _serverRegistrations = new();
    private readonly ConcurrentDictionary<long, List<McpServerRunner>> _runners = new();
    private readonly ILogger<McpServerRunnerManager> _logger;

    public McpServerRunnerManager(ILogger<McpServerRunnerManager> logger)
    {
        _logger = logger;
    }

    public async Task<List<McpAIFunction>> GetMcpToolsForUserAsync(ClaimsPrincipal principal, long projectId, CancellationToken cancellationToken = default)
    {
        var userName = principal.Identity?.Name ?? throw new ArgumentNullException(nameof(principal.Identity.Name), "User identity is required to get MCP tools.");

        List<McpAIFunction> tools = [];
        foreach (var registration in _serverRegistrations.Values)
        {
            if (_runners.TryGetValue(registration.Id, out var runners))
            {
                foreach (var runner in runners)
                {
                    if (runner.Registration.TestProjectId == projectId)
                    {
                        if (userName == runner.Registration.CreatedBy || runner.Registration.PublicForProject)
                        {
                            tools.AddRange(await runner.GetToolsForSessionAsync(userName, cancellationToken));
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
                server.ErrorMessage = null;
                _runners[registration.Id].Add(runner);
            }
            catch(Exception ex)
            {
                server.ErrorMessage = ex.Message;
            }
        }
    }

    public async Task RestartServerAsync(McpServerRegistration registration, CancellationToken cancellationToken)
    {
        if (registration == null)
        {
            throw new ArgumentNullException(nameof(registration), "MCP server registration cannot be null.");
        }
        if (!_serverRegistrations.ContainsKey(registration.Id))
        {
            throw new InvalidOperationException($"MCP server registration with ID {registration.Id} is not running.");
        }
        await StopServerAsync(registration, cancellationToken);
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
