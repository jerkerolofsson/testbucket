using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

namespace TestBucket.Domain.AI.Agent.Agents;
internal class RequirementReviewerAgent
{
    public static ChatCompletionAgent Create(Kernel kernel, Microsoft.Extensions.Logging.ILoggerFactory loggerFactory)
    {
        return new ChatCompletionAgent()
        {
            LoggerFactory = loggerFactory,
            Description = "A requirement reviewer",
            Instructions =
            """
            You are a requirement analyst.
            Your goal is to review requirements and make sure that they are specific and contains required information.
            Requirements must contain a title and a description.

            If the requirement contains all information, state that it is approved. 
            
            If not, provide insight on how to refine suggested test case with the following example:
            
            # Rules
            - Never ask for user input

            # EXAMPLE
            
            ## Title

            An error message should be displayed if the file is corrupt.

            ## Description

            If the user uploads a corrupt file, an error message should be displayed saying that the file is corrupt and 
            could not be parsed.

            """,
            Name = "Requirement-Reviewer",
            Kernel = kernel,
            Arguments = new KernelArguments(new PromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            })
        };
    }
}
