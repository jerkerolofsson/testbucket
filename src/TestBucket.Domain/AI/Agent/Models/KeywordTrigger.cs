using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.AI;

namespace TestBucket.Domain.AI.Agent.Models;

/// <summary>
/// This pipeline trigger runs when the user prompt contain a specific keyword
/// </summary>
internal class KeywordTrigger
{
    public required string Keyword { get; set; }
    public required string Prompt { get; set; }
    public required string UserMessage { get; set; }

    public async IAsyncEnumerable<ChatResponseUpdate> RunAsync(ClaimsPrincipal principal,
        AgentChatContext context,
        ChatMessage userMessage,
        List<ChatMessage> contextMessages,
        IChatClient chatClient,
        AgentChatClient client,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach(var update in client.CallModelAsync(principal, context, userMessage, contextMessages, chatClient, cancellationToken))
        {
            yield return update;
        }
    }
}
