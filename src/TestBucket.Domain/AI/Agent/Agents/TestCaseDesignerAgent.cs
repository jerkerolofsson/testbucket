
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

namespace TestBucket.Domain.AI.Agent.Agents;
internal class TestCaseDesignerAgent
{
    public static ChatCompletionAgent Create(Kernel kernel, ILoggerFactory loggerFactory)
    {
        return new ChatCompletionAgent()
        {
            LoggerFactory = loggerFactory,
            Description = "A test designer",
            Instructions =
            """
            You are a test designer.

            Your goal is to draft test cases based on the information (features, requirements, specifications) in the message history.
            Get hueristics from the get_hueristics tool
            Use relevant heuristics in combination with the earlier messages to create test ideas

            # Rules
            - Don't add any test cases using tools. Just create drafts.
            - Never ask for user input
            """,
            Name = "Test-Designer",
            Kernel = kernel,
            Arguments = new KernelArguments(new PromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            })
        };
    }
}
