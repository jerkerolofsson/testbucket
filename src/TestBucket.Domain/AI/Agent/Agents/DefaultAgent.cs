
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

namespace TestBucket.Domain.AI.Agent.Agents;
internal class DefaultAgent
{
    public static ChatCompletionAgent Create(Kernel kernel)
    {
        return new ChatCompletionAgent()
        {
            Description = "Helpful assistant",
            Instructions =
            """
            You are a software expert who will help with all tasks related to software development but with a strong
            focus on Quality Assurance and testing.

            You never answer with code unless explicitly asked to do so.
            """,
            Name = "QA Support Agent",
            Kernel = kernel,
            Arguments = new KernelArguments(new PromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            })
        };
    }
}
