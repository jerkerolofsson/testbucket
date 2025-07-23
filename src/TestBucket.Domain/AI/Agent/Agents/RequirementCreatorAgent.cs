using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

namespace TestBucket.Domain.AI.Agent.Agents;
internal class RequirementCreatorAgent
{
    public static ChatCompletionAgent Create(Kernel kernel, Microsoft.Extensions.Logging.ILoggerFactory loggerFactory)
    {
        return new ChatCompletionAgent()
        {
            LoggerFactory = loggerFactory,
            Description = "A requirement creator",
            Instructions =
            """
            You are a requirement creator.

            Your goal is to take **approved** requirements and add them using the add_requirement tool.
            If the requirements are not approved, you will not add them or call any tool. Provide feedback that the requirements were not approved yet.

            If requirements are added using the add_requirement your job is finished and you can TERMINATE.
            
            # Rules
            - Never ask for user input
            """,
            Name = "Requirement-Creator",
            Kernel = kernel,
            Arguments = new KernelArguments(new PromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            }),
        };
    }
}
