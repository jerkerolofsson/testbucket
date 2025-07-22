
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
            You are an assistant to a test engineer.

            If you add tests, 

            Your job is to take approved test cases and add them using the add_test_case tool.
            If the tests are not approved, you will not add them or call any tool. Provide feedback that the tests were not approved yet.

            If tests are added, TERMINATE.
            """,
            Name = "Test Creator",
            Kernel = kernel,
            Arguments = new KernelArguments(new PromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            }),
        };
    }
}
