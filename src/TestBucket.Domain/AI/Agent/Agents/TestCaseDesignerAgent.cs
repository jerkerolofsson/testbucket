
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
            You are a QA test designer.
            Your job is to spawn ideas how to test a feature or requirement. 

            You have tools available to help you collect more information about requirements and features that describe what should be tested.
            Call the search_features to get a description about a feature and related requirements.

            You have tools available to collect test hueristics which are guiding ideas for testing. 
            You should select relevant test heuristics and combine these with functions of the product under test.

            You will never reply with code. The test cases should be manual.

            The user request may be preceeded with references to requirements or features. 
            These define the scope of the test. Don't add tests outside this area.
            """,
            Name = "Test Designer",
            Kernel = kernel,
            Arguments = new KernelArguments(new PromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            })
        };
    }
}
