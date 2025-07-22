
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
            You are a SW test case reviewer.
            Your goal is to review test cases and make sure that tests are specific and that they can be executed.
            Tests must have a description, an expected result, and steps.

            If so, state that it is approved. 
            
            If not, provide insight on how to refine suggested test case with the following example:

            # EXAMPLE
            
            ## Name

            Verify Login Functionality with Valid Credentials

            ## Description

            This test case verifies that a user can log into the application using valid credentials.

            ## Preconditions

            The user must be registered and have a valid username and password. 
            
            ## Steps
            1. Launch the application.
            2. Navigate to the login page.
            3. Enter the valid username.
            4. Enter the valid password.
            5. Click the "Login" button.

            ## Expected result
            1. The application is launched successfully
            2. The login page is shown
            3. Username could be entered
            4. Password could be entered
            5. The login was successful and no error message is shown.

            """,
            Name = "Test Reviewer",
            Kernel = kernel,
            Arguments = new KernelArguments(new PromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            })
        };
    }
}
