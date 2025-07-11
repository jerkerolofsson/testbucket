using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.AI.Mcp.Models;
using TestBucket.Domain.AI.Mcp.Services;

namespace TestBucket.Domain.AI.Mcp.Events;
public record class McpServerAdded(McpServerRegistration Registration) : INotification;

public class StartMcpServerWhenAdded : INotificationHandler<McpServerAdded>
{
    private readonly McpServerRunnerManager _manager;

    public StartMcpServerWhenAdded(McpServerRunnerManager manager)
    {
        _manager = manager;
    }

    public async ValueTask Handle(McpServerAdded notification, CancellationToken cancellationToken)
    {
        await _manager.StartServerAsync(notification.Registration, cancellationToken);
    }
}