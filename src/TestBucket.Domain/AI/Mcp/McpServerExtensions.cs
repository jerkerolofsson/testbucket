using Microsoft.Extensions.DependencyInjection;

namespace TestBucket.Domain.AI.Mcp;
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
