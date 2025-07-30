using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents.Magentic;
using Microsoft.SemanticKernel.Agents.Orchestration.Extensions;
using Microsoft.SemanticKernel.Agents.Orchestration.GroupChat;
using Microsoft.SemanticKernel.Agents.Runtime;
using Microsoft.SemanticKernel.ChatCompletion;

namespace TestBucket.Domain.AI.Agent.Orchestration;

internal class GroupChatOrchestrationWithChatHistory :  GroupChatOrchestration<string, string>
{
    private readonly ChatHistory _chatHistory;

    public GroupChatOrchestrationWithChatHistory(ChatHistory chatHistory, GroupChatManager manager, params Microsoft.SemanticKernel.Agents.Agent[] agents) : base(manager, agents)
    {
        _chatHistory = chatHistory;
    }

    protected override async ValueTask StartAsync(IAgentRuntime runtime, TopicId topic, IEnumerable<ChatMessageContent> input, AgentType? entryAgent)
    {
        if (!entryAgent.HasValue)
        {
            throw new ArgumentException("Entry agent is not defined.", nameof(entryAgent));
        }
        _chatHistory.AddRange(input);
        await base.StartAsync(runtime, topic, _chatHistory, entryAgent);

    }
}
