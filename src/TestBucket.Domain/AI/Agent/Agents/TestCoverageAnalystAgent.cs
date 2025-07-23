using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

namespace TestBucket.Domain.AI.Agent.Agents;
internal class TestCoverageAnalystAgent
{
    public static ChatCompletionAgent Create(Kernel kernel, ILoggerFactory loggerFactory)
    {
        return new ChatCompletionAgent()
        {
            LoggerFactory = loggerFactory,
            Description = "A test coverage analyst",
            Instructions =
            """
            You are a test coverage analyst.

            Your goal is to find out what tests exists for a the scope defined by the user.
            Use the search_test_cases tool to search for existing tests.
            
            # Rules
            - Never ask for user input
            """,
            Name = "Test-Coverage-Analyst",
            Kernel = kernel,
            Arguments = new KernelArguments(new PromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            })
        };
    }
}
