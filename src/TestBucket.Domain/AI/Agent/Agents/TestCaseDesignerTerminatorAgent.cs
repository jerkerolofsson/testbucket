
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

namespace TestBucket.Domain.AI.Agent.Agents;
internal class TestCaseDesignerTerminatorAgent
{
    public static ChatCompletionAgent Create(Kernel kernel)
    {
        return new ChatCompletionAgent()
        {
            Description = "A test idea guard",
            Instructions =
            """
            If test ideas are created and they are approved: TERMINATE.
            """,
            Name = "Test Idea Guard",
            Kernel = kernel,
            Arguments = new KernelArguments(new PromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            })
        };
    }
}
