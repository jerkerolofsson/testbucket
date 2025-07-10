using Microsoft.Extensions.DependencyInjection;

using TestBucket.Domain.AI.Mcp.Tools;

namespace TestBucket.Domain.AI.Mcp.Extensions;
public static class McpServerExtensions
{
    public static IServiceCollection AddMcpTools(this IServiceCollection services)
    {
        services.AddMcpServer().WithHttpTransport(httpTransportOptions =>
        {
            httpTransportOptions.RunSessionHandler = async (httpContext, mcpServer, cancellationToken) =>
            {
                AuthenticatedTool.AuthorizationHeader.Value = httpContext.Request.Headers["Authorization"];
                await mcpServer.RunAsync(cancellationToken);
            };
            httpTransportOptions.Stateless = false; // We need stateful for the authentication to work
        })
       .WithToolsFromAssembly(typeof(WhoAmITool).Assembly);
        
        return services;
    }
}
