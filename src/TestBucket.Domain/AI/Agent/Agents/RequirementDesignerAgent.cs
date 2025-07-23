
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

namespace TestBucket.Domain.AI.Agent.Agents;
internal class RequirementDesignerAgent
{
    public static ChatCompletionAgent Create(Kernel kernel, Microsoft.Extensions.Logging.ILoggerFactory loggerFactory)
    {
        return new ChatCompletionAgent()
        {
            LoggerFactory = loggerFactory,
            Description = "A requirement designer",
            Instructions =
            """
            You are a requirement designer.

            Your job is to design requirements based on the information available in the context.
            You are constrained to the area defined by the user prompt. You should not consider any data outside this scope.

            If the available data is not sufficient to create a requirement, you should ask for more information or clarification.
            You will look at the available data and write requirements based on the information you find.

            A requirement must have a title and a description. 
            The title can be a short summary of the requirement, while the description should provide detailed information about what the requirement entails.
            """,
            Name = "Requirement-Designer",
            Kernel = kernel,
            Arguments = new KernelArguments(new PromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            })
        };
    }
}
