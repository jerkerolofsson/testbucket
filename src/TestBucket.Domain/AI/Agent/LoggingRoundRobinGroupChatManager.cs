using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.Agents.Orchestration.GroupChat;
using Microsoft.SemanticKernel.ChatCompletion;

namespace TestBucket.Domain.AI.Agent;

#pragma warning disable SKEXP0110
#pragma warning disable SKEXP0001
internal class LoggingRoundRobinGroupChatManager : RoundRobinGroupChatManager
{
    private readonly ILogger _logger;

    public LoggingRoundRobinGroupChatManager(ILogger logger)
    {
        _logger = logger;
    }

    public override ValueTask<GroupChatManagerResult<bool>> ShouldTerminate(ChatHistory history, CancellationToken cancellationToken = default)
    {
        foreach(var message in history)
        {
            if(message.Role == AuthorRole.Assistant)
            {
                if(message.Content is not null)
                {
                    if(message.Content.Contains("TERMINATE"))
                    {
                        var result = new GroupChatManagerResult<bool>(true);
                        _logger.LogInformation("Agent: {AuthorName}, requested termination from content: {Content}", message.AuthorName, message.Content);
                        return ValueTask.FromResult(result);
                    }
                }
            }
        }

        return base.ShouldTerminate(history, cancellationToken);
    }

    public override async ValueTask<GroupChatManagerResult<string>> SelectNextAgent(ChatHistory history, GroupChatTeam team, CancellationToken cancellationToken = default)
    {
        var result = await base.SelectNextAgent(history, team, cancellationToken);

        _logger.LogInformation($"Next agent: {result.Value}, reason={result.Reason}");

        return result;
    }
}
