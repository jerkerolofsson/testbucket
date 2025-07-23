using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Orchestration;
using Microsoft.SemanticKernel.Agents.Orchestration.Sequential;
using Microsoft.SemanticKernel.ChatCompletion;

namespace TestBucket.Domain.AI.Agent.Orchestration;
internal class OrchestrationHelper
{
    public static SequentialOrchestrationWithChatHistory CreateSequential(
        ChatHistory chatHistory,
        IEnumerable<ChatCompletionAgent> members,
        OrchestrationResponseCallback responseCallback,
        OrchestrationStreamingCallback streamingCallback,
        IServiceProvider serviceProvider)
        => new SequentialOrchestrationWithChatHistory(chatHistory, members.ToArray())
        {
            ResponseCallback = responseCallback,
            StreamingResponseCallback = streamingCallback
        };

    public static GroupChatOrchestrationWithChatHistory CreateGroupChat(
        ChatHistory chatHistory, 
        IEnumerable<ChatCompletionAgent> members, 
        OrchestrationResponseCallback responseCallback, 
        OrchestrationStreamingCallback streamingCallback,
        IServiceProvider serviceProvider,
        int? maximumInvocationCount = null )
    {
        var chatManager = new LoggingRoundRobinGroupChatManager(serviceProvider.GetRequiredService<ILogger<LoggingRoundRobinGroupChatManager>>())
        {
            // Default
            MaximumInvocationCount = maximumInvocationCount ?? (members.Count() * 2),
        };

        var orchestration = new GroupChatOrchestrationWithChatHistory(chatHistory, chatManager, members.ToArray())
        {
            ResponseCallback = responseCallback,
            StreamingResponseCallback = streamingCallback
        };
        return orchestration;
    }
}
