using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

using TestBucket.Domain.AI.Agent.Agents;

namespace TestBucket.Domain.AI.Agent.Orchestration;
internal class AddTestsOrchestration : IOrchestrationMemberBuilder
{
    public ChatCompletionAgent[] GetMembers(Kernel kernel, ILoggerFactory loggerFactory)
    {
        return [
            RequirementAnalystAgent.Create(kernel, loggerFactory),
                TestCoverageAnalystAgent.Create(kernel, loggerFactory),

                // Design/Draft
                TestCaseDesignerAgent.Create(kernel, loggerFactory),

                // Review
                TestReviewerAgent.Create(kernel, loggerFactory),

                // Add
                TestCreatorAgent.Create(kernel, loggerFactory)
                ];
    }
}
