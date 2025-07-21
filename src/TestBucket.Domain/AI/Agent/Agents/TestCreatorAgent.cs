
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

namespace TestBucket.Domain.AI.Agent.Agents;
internal class TestCreatorAgent
{
    public static ChatCompletionAgent Create(Kernel kernel)
    {
        return new ChatCompletionAgent()
        {
            Description = "A test creator",
            Instructions =
            """
            You are a test creator.
            Your job is to take approved test cases and add them using the add-test-case tool.
            If tests are added, TERMINATE.
            """,
            Name = "Test Creator",
            Kernel = kernel,
            Arguments = new KernelArguments(new PromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            })
        };
    }
}
