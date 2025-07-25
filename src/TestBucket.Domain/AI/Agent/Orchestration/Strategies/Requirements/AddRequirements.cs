using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Orchestration;
using Microsoft.SemanticKernel.ChatCompletion;

using TestBucket.Domain.AI.Agent.Agents;

namespace TestBucket.Domain.AI.Agent.Orchestration.Strategies.Requirements;
internal class AddRequirements : IOrchestrationStrategy
{
    public string Name => OrchestrationStrategies.AddRequirements;

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
                // Analyze inputs
                RequirementAnalystAgent.Create(kernel, loggerFactory),
                TestCoverageAnalystAgent.Create(kernel, loggerFactory),

                // Design/Draft
                RequirementDesignerAgent.Create(kernel, loggerFactory),

                // Review
                RequirementReviewerAgent.Create(kernel, loggerFactory),

                // Add
                RequirementCreatorAgent.Create(kernel, loggerFactory)
            ];
    }
}