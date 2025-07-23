using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Orchestration;
using Microsoft.SemanticKernel.ChatCompletion;

using TestBucket.Domain.AI.Agent.Agents;

namespace TestBucket.Domain.AI.Agent.Orchestration.Strategies;
internal class DraftTestsStrategy : IOrchestrationStrategy
{
    public string Name => OrchestrationStrategies.DraftTests;

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
                RequirementAnalystAgent.Create(kernel, loggerFactory),
                TestCoverageAnalystAgent.Create(kernel, loggerFactory),

                // Design/Draft
                TestCaseDesignerAgent.Create(kernel, loggerFactory),

                // Review
                TestReviewerAgent.Create(kernel, loggerFactory),
            ];
    }
}