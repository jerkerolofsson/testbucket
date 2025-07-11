using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.AI.Mcp.Models;
using TestBucket.Domain.AI.Mcp.Services;

namespace TestBucket.Domain.AI.Mcp.Events;
public record class McpServerRemoved(McpServerRegistration Registration) : INotification;

public class StartMcpServerWhenRemoved : INotificationHandler<McpServerRemoved>
{
    private readonly McpServerRunnerManager _manager;

    public StartMcpServerWhenRemoved(McpServerRunnerManager manager)
    {
        _manager = manager;
    }

    public async ValueTask Handle(McpServerRemoved notification, CancellationToken cancellationToken)
    {
        await _manager.StopServerAsync(notification.Registration, cancellationToken);
    }
}