using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

namespace TestBucket.Domain.AI.Agent.Agents;
internal class AiRunnerResultEvaluatorAgent
{
    public static ChatCompletionAgent Create(Kernel kernel, Microsoft.Extensions.Logging.ILoggerFactory loggerFactory)
    {
        return new ChatCompletionAgent()
        {
            LoggerFactory = loggerFactory,
            Description = "AI Runner Result evaluator",
            Instructions =
            """
            You are a tester.

            Your goal is to evaluate if a test is passed or failed.

            # Rules
            - You must not call any tools

            # Steps
            1. Evaluate the test result from the context
            2. Output conclusion based on the evaluation
            3. Output "TERMINATE"

            # Conclusion
            - If a problem or a failure is detected. Respond with "FAILED". Summarize what the problem is.
            - If all steps are executed successfully, respond with "PASSED".
            - If the result cannot be determined, respond with "INCONCLUSIVE"
            """,
            Name = "ai-runner-evaluator",
            Kernel = kernel,
            Arguments = new KernelArguments(new PromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            })
        };
    }
}
