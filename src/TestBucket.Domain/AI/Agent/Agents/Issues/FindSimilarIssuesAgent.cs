using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

namespace TestBucket.Domain.AI.Agent.Agents.Issues;
internal class FindSimilarIssuesAgent
{
    public static ChatCompletionAgent Create(Kernel kernel, Microsoft.Extensions.Logging.ILoggerFactory loggerFactory)
    {
        return new ChatCompletionAgent()
        {
            LoggerFactory = loggerFactory,
            Description = "Issue researcher",
            Instructions =
            """
            You are a QA specializing in issue/bug/ticket research.

            Your goal is to find similar issue to the issue that was specified in the context.

            # Rules
            - Use the search_issues tool to find similar issues. Search based on relevant keywords, titles, and descriptions.
            - Don't make up issues.
            """,
            Name = "issue-researcher",
            Kernel = kernel,
            Arguments = new KernelArguments(new PromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            })
        };
    }
}
