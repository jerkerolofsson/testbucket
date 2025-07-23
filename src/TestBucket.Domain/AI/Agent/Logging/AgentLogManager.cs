using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.SemanticKernel;

namespace TestBucket.Domain.AI.Agent.Logging;
internal class AgentLogManager : IAgentLogManager
{
    public ValueTask LogResponseAsync(ClaimsPrincipal principal, ChatMessageContent response)
    {
        if (response.Metadata is not null && response.Metadata.TryGetValue("Usage", out var usage))
        {
            string? modelId = response.ModelId;
            if (usage is OpenAI.Chat.ChatTokenUsage openAiUsage)
            {

            }
        }
        return ValueTask.CompletedTask;
    }
}
