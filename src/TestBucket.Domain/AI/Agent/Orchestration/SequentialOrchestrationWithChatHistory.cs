using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents.Magentic;
using Microsoft.SemanticKernel.Agents.Orchestration.Extensions;
using Microsoft.SemanticKernel.Agents.Orchestration.GroupChat;
using Microsoft.SemanticKernel.Agents.Orchestration.Sequential;
using Microsoft.SemanticKernel.Agents.Runtime;
using Microsoft.SemanticKernel.ChatCompletion;

namespace TestBucket.Domain.AI.Agent;

internal class SequentialOrchestrationWithChatHistory :  SequentialOrchestration<string, string>
{
    private readonly ChatHistory _chatHistory;

    public SequentialOrchestrationWithChatHistory(ChatHistory chatHistory, params Microsoft.SemanticKernel.Agents.Agent[] agents) : base(agents)
    {
        _chatHistory = chatHistory;
    }

    protected override async ValueTask StartAsync(IAgentRuntime runtime, TopicId topic, IEnumerable<ChatMessageContent> input, AgentType? entryAgent)
    {
        if (!entryAgent.HasValue)
        {
            throw new ArgumentException("Entry agent is not defined.", nameof(entryAgent));
        }
        _chatHistory.AddRange(input); // This will be an input from InvokeAsync
        await base.StartAsync(runtime, topic, _chatHistory, entryAgent);
    }
}
