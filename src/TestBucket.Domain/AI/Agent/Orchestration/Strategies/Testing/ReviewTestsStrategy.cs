using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Orchestration;
using Microsoft.SemanticKernel.ChatCompletion;

using TestBucket.Domain.AI.Agent.Agents;

namespace TestBucket.Domain.AI.Agent.Orchestration.Strategies.Testing;
internal class ReviewTestsStrategy : IOrchestrationStrategy
{
    public string Name => OrchestrationStrategies.ReviewTests;

    public AgentOrchestration<string,string> CreateOrchestration(
        ChatHistory history, 
        Kernel kernel, 
        IServiceProvider serviceProvider,
        OrchestrationResponseCallback responseCallback,
        OrchestrationStreamingCallback streamingCallback)
    {
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var members = GetMembers(kernel, loggerFactory);
        return OrchestrationHelper.CreateSequential(history, members, responseCallback, streamingCallback, serviceProvider);
    }

    public ChatCompletionAgent[] GetMembers(Kernel kernel, ILoggerFactory loggerFactory)
    {
        return [TestReviewerAgent.Create(kernel, loggerFactory)];
    }
}
