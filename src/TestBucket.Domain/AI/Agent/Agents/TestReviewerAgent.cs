
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

namespace TestBucket.Domain.AI.Agent.Agents;
internal class TestReviewerAgent
{
    public static ChatCompletionAgent Create(Kernel kernel, Microsoft.Extensions.Logging.ILoggerFactory loggerFactory)
    {
        return new ChatCompletionAgent()
        {
            LoggerFactory = loggerFactory,
            Description = "A test reviewer",
            Instructions =
            """
            You are a test reviewer.

            Your goal is to review the test cases in the chat history and make sure that tests are specific and that they can be executed.
            If tests contain the required properties, state that it is APPROVED. 
            If not, provide insight on what is missing.
            
            # Rules
            - Tests must have a description
            - Tests must have an expected result
            - Tests must have a name 
            - Tests must have steps describing how to test
            - Never ask for user input
            """,
            Name = "Test-Reviewer",
            Kernel = kernel,
            Arguments = new KernelArguments(new PromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            })
        };
    }
}
