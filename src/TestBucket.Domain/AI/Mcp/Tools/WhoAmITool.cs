﻿using System.ComponentModel;
using System.Xml.Linq;

using ModelContextProtocol.Server;

using TestBucket.Domain.ApiKeys;

namespace TestBucket.Domain.AI.Mcp.Tools;

/// <summary>
/// This tool returns the name of the user that is authenticated with the API key.
/// It is useful to debug to verify that authentication is working correctly with the MCP client configuration.
/// </summary>
[McpServerToolType()]
[DisplayName("system")]
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
    [McpServerTool(Name = "who_am_i"), Description("Returns the name of the user")]
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