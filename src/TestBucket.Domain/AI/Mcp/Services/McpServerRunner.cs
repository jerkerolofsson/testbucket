using System.Collections.Concurrent;

using Microsoft.SemanticKernel;

using ModelContextProtocol.Client;

using TestBucket.Domain.AI.Mcp.Models;
using TestBucket.Domain.AI.Mcp.Tools;

namespace TestBucket.Domain.AI.Mcp.Services;

/// <summary>
/// Manages the lifecycle and client connections for a Model Context Protocol (MCP) server instance.
/// Handles client creation, connection management, and tool retrieval for user sessions.
/// </summary>
internal class McpServerRunner : IAsyncDisposable
{
    private readonly McpServerRegistration _registration;
    private readonly McpServer _configuration;
    private readonly string _name;
    private IMcpClient? _client;
    private readonly ConcurrentDictionary<string, IMcpClient> _userClients = [];
    private readonly ConcurrentDictionary<string, IList<McpAIFunction>> _userTools = [];

    /// <summary>
    /// Gets the server registration information.
    /// </summary>
    public McpServerRegistration Registration => _registration;

    /// <summary>
    /// Gets the name of the MCP server.
    /// </summary>
    public string Name => _name;

    public string McpToolName => _configuration.ToolName ?? _name;

    /// <summary>
    /// Creates and initializes a new MCP server runner instance.
    /// </summary>
    /// <param name="registration">The server registration containing database ID and configuration.</param>
    /// <param name="configuration">The MCP server configuration with connection details.</param>
    /// <param name="name">The display name for the server.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A fully initialized MCP server runner.</returns>
    /// <exception cref="Exception">Thrown when the server type is not supported or configuration is invalid.</exception>
    public static async Task<McpServerRunner> CreateAsync(McpServerRegistration registration, McpServer configuration, string name, CancellationToken cancellationToken)
    {
        var runner = new McpServerRunner(registration, configuration, name);
        await runner.CreateClientAsync(cancellationToken);
        return runner;
    }

    /// <summary>
    /// Initializes a new instance of the McpServerRunner class.
    /// </summary>
    /// <param name="registration">The server registration containing database ID and configuration.</param>
    /// <param name="configuration">The MCP server configuration with connection details.</param>
    /// <param name="name">The display name for the server.</param>
    private McpServerRunner(McpServerRegistration registration, McpServer configuration, string name)
    {
        _registration = registration;
        _configuration = configuration;
        _name = name;
    }

    /// <summary>
    /// Creates and configures the main MCP client connection based on the server configuration.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <exception cref="Exception">Thrown when the server type is not supported or URL is missing for HTTP-based connections.</exception>
    private async Task CreateClientAsync(CancellationToken cancellationToken)
    {
        if (_client is not null)
        {
            await _client.DisposeAsync();
        }

        if (_configuration.Type == "stdio")
        {
            throw new Exception("stdio servers are not supported.");
        }
        else if ((_configuration.Type is "sse" or "http" or "https") || _configuration.Url is not null)
        {
            if (_configuration.Url is null)
            {
                throw new Exception("url is required when type is " + _configuration.Type);
            }

            IMcpClient client = await CreateClientAsync();
            _client = client;

            // Verify that we can connect
            var tools = await _client.ListToolsAsync(null, cancellationToken);
        }
        else
        {
            throw new Exception("Unknown MCP client type");
        }
    }

    /// <summary>
    /// Creates a new MCP client instance with the configured transport settings.
    /// </summary>
    /// <returns>A configured MCP client ready for communication.</returns>
    private async Task<IMcpClient> CreateClientAsync()
    {
        HttpTransportMode transportMode = _configuration.Type switch
        {
            "sse" => HttpTransportMode.Sse,
            "http" => HttpTransportMode.StreamableHttp,
            "https" => HttpTransportMode.StreamableHttp,
            _ => HttpTransportMode.AutoDetect
        };

        var options = new SseClientTransportOptions
        {
            Endpoint = new Uri(_configuration.Url!),
            AdditionalHeaders = _configuration.Headers?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) ?? new Dictionary<string, string>(),
            Name = _name,
            TransportMode = transportMode,
            ConnectionTimeout = TimeSpan.FromSeconds(10)
        };
        var transport = new SseClientTransport(options);
        var client = await McpClientFactory.CreateAsync(transport);
        return client;
    }

    /// <summary>
    /// Restarts the MCP server connection by recreating the client.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <exception cref="Exception">Thrown when the server type is not supported or configuration is invalid.</exception>
    public async Task RestartAsync(CancellationToken cancellationToken)
    {
        await CreateClientAsync(cancellationToken);
    }

    /// <summary>
    /// Disposes all client connections and clears cached data.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_client is not null)
        {
            await _client.DisposeAsync();
        }
        foreach (var userClient in _userClients.Values)
        {
            await userClient.DisposeAsync();
        }
        _userClients.Clear();
        _userTools.Clear();
    }

    /// <summary>
    /// Retrieves the available AI functions/tools for a specific user session.
    /// Creates a new client connection for the session if one doesn't exist.
    /// </summary>
    /// <param name="sessionId">The unique identifier for the user session.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of AI functions available from the MCP server for the session.</returns>
    internal async Task<IEnumerable<McpAIFunction>> GetToolsForSessionAsync(string sessionId, CancellationToken cancellationToken)
    {
        if (_userTools.TryGetValue(sessionId, out var existingTools))
        {
            return existingTools;
        }

        IMcpClient client = await CreateClientAsync();
        _userClients[sessionId] = client;

        // Verify that we can connect
        var tools = await client.ListToolsAsync(null, cancellationToken);
        var userTools = tools.Select(x => new McpAIFunction
        {
            McpServerRegistration = _registration,
            McpServer = _configuration,
            Tool = x,
            AIFunction = x
        }).ToList();

        _userTools[sessionId] = userTools;

        return userTools;

    }
}