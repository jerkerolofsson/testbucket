using Mediator;

using TestBucket.Domain.AI.Mcp.Models;
using TestBucket.Domain.AI.Mcp.Services;

namespace TestBucket.Domain.AI.Mcp.Events;
public record class McpServerUpdated(McpServerRegistration Registration) : INotification;

public class StartMcpServerWhenUpdated : INotificationHandler<McpServerUpdated>
{
    private readonly McpServerRunnerManager _manager;

    public StartMcpServerWhenUpdated(McpServerRunnerManager manager)
    {
        _manager = manager;
    }

    public async ValueTask Handle(McpServerUpdated notification, CancellationToken cancellationToken)
    {
        await _manager.RestartServerAsync(notification.Registration, cancellationToken);
    }
}