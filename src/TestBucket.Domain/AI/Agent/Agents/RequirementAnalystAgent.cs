using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

namespace TestBucket.Domain.AI.Agent.Agents;
internal class RequirementAnalystAgent
{
    public static ChatCompletionAgent Create(Kernel kernel, ILoggerFactory loggerFactory)
    {
        return new ChatCompletionAgent()
        {
            LoggerFactory = loggerFactory,
            Description = "A requirement analyst",
            Instructions =
            """
            You are a requirement analyst.
            Your job is to collect information about requirements and features and to provide a summary of what requirements exists and how a feature or function should work.

            Use the search_features tool to search for features.
            Use the search_requirements tool to search for requirement.
            """,
            Name = "Requirement Analyst",
            Kernel = kernel,
            Arguments = new KernelArguments(new PromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            })
        };
    }
}
