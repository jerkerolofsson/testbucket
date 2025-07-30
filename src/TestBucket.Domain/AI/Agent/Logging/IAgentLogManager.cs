
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;

using TestBucket.Domain.AI.Billing;

namespace TestBucket.Domain.AI.Agent.Logging;
internal interface IAgentLogManager
{
    ValueTask<ChatUsage?> LogResponseAsync(long? teamId, long? projectId, string orchestrationStrategy, ClaimsPrincipal principal, ChatMessageContent response);
    ValueTask<ChatUsage?> LogResponseAsync(long? teamId, long? projectId, ClaimsPrincipal principal, ChatResponseUpdate update);
}