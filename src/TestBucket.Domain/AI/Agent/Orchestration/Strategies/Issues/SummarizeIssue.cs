using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Orchestration;
using Microsoft.SemanticKernel.ChatCompletion;

using TestBucket.Domain.AI.Agent.Agents;
using TestBucket.Domain.AI.Agent.Agents.Issues;

namespace TestBucket.Domain.AI.Agent.Orchestration.Strategies;
internal class SummarizeIssue : IOrchestrationStrategy
{
    public string Name => OrchestrationStrategies.SummarizeIssue;

    public AgentOrchestration<string, string> CreateOrchestration(
        ChatHistory history,
        Kernel kernel,
        IServiceProvider serviceProvider,
        OrchestrationResponseCallback responseCallback,
        OrchestrationStreamingCallback streamingCallback)
    {
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var members = GetMembers(kernel, loggerFactory);
        return OrchestrationHelper.CreateGroupChat(history, members, responseCallback, streamingCallback, serviceProvider);
    }

    public ChatCompletionAgent[] GetMembers(Kernel kernel, ILoggerFactory loggerFactory)
    {
        return 
            [
                IssueSummarizerAgent.Create(kernel, loggerFactory),
            ];
    }
}