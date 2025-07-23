
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;

namespace TestBucket.Domain.AI.Agent.Logging;
internal interface IAgentLogManager
{
    ValueTask LogResponseAsync(long? teamId, long? projectId, string orchestrationStrategy, ClaimsPrincipal principal, ChatMessageContent response);
    ValueTask LogResponseAsync(long? teamId, long? projectId, ClaimsPrincipal principal, ChatResponseUpdate update);
}