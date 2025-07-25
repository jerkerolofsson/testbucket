using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

namespace TestBucket.Domain.AI.Agent.Agents.Issues;
internal class IssueSummarizerAgent
{
    public static ChatCompletionAgent Create(Kernel kernel, Microsoft.Extensions.Logging.ILoggerFactory loggerFactory)
    {
        return new ChatCompletionAgent()
        {
            LoggerFactory = loggerFactory,
            Description = "Issue specialist",
            Instructions =
            """
            You are a SW engineer specializing in issue/bug/ticket analysis.

            Your goal is to analyze the specified issue and provide a short summary with the most important information.

            # Rules
            - Don't make up issues.
            - Don't use the list_open_issues tool.
            - You may search for related requirements using the search_requirements tool.
            """,
            Name = "issue-specialist",
            Kernel = kernel,
            Arguments = new KernelArguments(new PromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            })
        };
    }
}
