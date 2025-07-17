using System.ComponentModel;

using ModelContextProtocol.Server;

using TestBucket.Domain.ApiKeys;

namespace TestBucket.Domain.AI.Mcp.Tools;

/// <summary>
/// </summary>
[McpServerToolType()]
public class DateAndTimeTool : AuthenticatedTool
{
    public DateAndTimeTool(IApiKeyAuthenticator apiKeyAuthenticator) : base(apiKeyAuthenticator)
    {
    }

    /// <summary>
    /// Returns the current date and time.
    /// </summary>
    /// <param name="server"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    [McpServerTool(Name = "get_current_time_and_date"), Description("Returns the current date and time.")]
    public string GetCurrentDateAndTime()
    {
        return DateTime.UtcNow.ToString("O"); 
    }
}