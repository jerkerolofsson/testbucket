using System.ComponentModel;

using ModelContextProtocol.Server;

using TestBucket.Domain.ApiKeys;

namespace TestBucket.Domain.AI.Mcp;

/// <summary>
/// This tool returns the name of the user that is authenticated with the API key.
/// It is useful to debug to verify that authentication is working correctly with the MCP client configuration.
/// </summary>
[McpServerToolType]
public class WhoAmITool : AuthenticatedTool
{
    public WhoAmITool(IApiKeyAuthenticator apiKeyAuthenticator) : base(apiKeyAuthenticator)
    {
    }

    /// <summary>
    /// Returns the name of the user that is authenticated with the API key.
    /// </summary>
    /// <param name="server"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    [McpServerTool, Description("Returns the name of the user")]
    public async Task<string> WhoAmIAsync()
    {
        var isAuthenticated = await IsAuthenticatedAsync();
        if (isAuthenticated)
        {
            return _principal?.Identity?.Name ?? "Unknown user";
        }
        return $"The user is not authenticated";
    }
}