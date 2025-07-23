using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Orchestration;
using Microsoft.SemanticKernel.Agents.Orchestration.Sequential;
using Microsoft.SemanticKernel.Agents.Runtime;

using TestBucket.Domain.AI.Agent.Agents;

#pragma warning disable SKEXP0001
#pragma warning disable SKEXP0110

namespace TestBucket.Domain.AI.Agent.Orchestration;
internal class OrchestrationBuilder
{
    public static AgentOrchestration<string,string> Build(
        string id,
        Kernel kernel, ILoggerFactory loggerFactory,
        OrchestrationResponseCallback responseCallback,
        OrchestrationStreamingCallback streamingCallback)
    {
        var members = GetMembers(id, kernel, loggerFactory);

        return CreateSequentialOrchestrator(members, responseCallback, streamingCallback, loggerFactory);
    }

    private static AgentOrchestration<string, string> CreateSequentialOrchestrator(List<ChatCompletionAgent> members, OrchestrationResponseCallback responseCallback, OrchestrationStreamingCallback streamingCallback, ILoggerFactory loggerFactory)
    {
        return new SequentialOrchestration(members.ToArray())
        {
            LoggerFactory = loggerFactory,
            ResponseCallback = responseCallback,
            StreamingResponseCallback = streamingCallback
        };
    }

    private static List<ChatCompletionAgent> GetMembers(string id, Kernel kernel, ILoggerFactory loggerFactory)
    {
        List<ChatCompletionAgent> members = [];
        switch (id)
        {
            case "test-designer":
                // Analyze inputs
                members.Add(RequirementAnalystAgent.Create(kernel, loggerFactory));
                members.Add(TestCoverageAnalystAgent.Create(kernel, loggerFactory));

                // Create test
                members.Add(TestCaseDesignerAgent.Create(kernel, loggerFactory));

                // Review
                members.Add(TestReviewerAgent.Create(kernel, loggerFactory));

                // Terminate
                //members.Add(TestCaseDesignerTerminatorAgent.Create(kernel, loggerFactory));
                break;

            case "requirement-designer":
                // Analyze inputs
                members.Add(RequirementAnalystAgent.Create(kernel, loggerFactory));
                members.Add(TestCoverageAnalystAgent.Create(kernel, loggerFactory));

                // Create
                members.Add(RequirementDesignerAgent.Create(kernel, loggerFactory));

                // Review
                members.Add(RequirementReviewerAgent.Create(kernel, loggerFactory));
                break;

            case "requirement-creator":
                // Analyze inputs
                members.Add(RequirementAnalystAgent.Create(kernel, loggerFactory));
                members.Add(TestCoverageAnalystAgent.Create(kernel, loggerFactory));

                // Design/Draft
                members.Add(RequirementDesignerAgent.Create(kernel, loggerFactory));

                // Review
                members.Add(RequirementReviewerAgent.Create(kernel, loggerFactory));

                // Add
                members.Add(RequirementCreatorAgent.Create(kernel, loggerFactory));
                break;

            case "test-creator":
                // Analyze inputs
                members.Add(RequirementAnalystAgent.Create(kernel, loggerFactory));
                members.Add(TestCoverageAnalystAgent.Create(kernel, loggerFactory));

                // Design/Draft
                members.Add(TestCaseDesignerAgent.Create(kernel, loggerFactory));

                // Review
                members.Add(TestReviewerAgent.Create(kernel, loggerFactory));

                // Add
                members.Add(TestCreatorAgent.Create(kernel, loggerFactory));
                break;

            default:
                members.Add(DefaultAgent.Create(kernel, loggerFactory));
                break;
        }
        return members;
    }
}
