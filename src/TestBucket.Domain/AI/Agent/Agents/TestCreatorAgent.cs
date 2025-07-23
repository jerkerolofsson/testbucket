
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

namespace TestBucket.Domain.AI.Agent.Agents;
internal class TestCreatorAgent
{
    public static ChatCompletionAgent Create(Kernel kernel, Microsoft.Extensions.Logging.ILoggerFactory loggerFactory)
    {
        return new ChatCompletionAgent()
        {
            LoggerFactory = loggerFactory,
            Description = "A test creator",
            Instructions =
            """
            You are a test creator.

            If tests are APPROVED:
            1. Add them using the add_test_case tool.
            2. Output "TERMINATE"

            If the tests are NOT APPROVED:
            1. Do not do anything.

            # Rules
            - Never ask for user input
            """,
            Name = "Test-Creator",
            Kernel = kernel,
            Arguments = new KernelArguments(new PromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            }),
        };
    }
}
