
using Microsoft.SemanticKernel;

namespace TestBucket.Domain.AI.Agent.Logging;
internal interface IAgentLogManager
{
    ValueTask LogResponseAsync(ClaimsPrincipal principal, ChatMessageContent response);
}