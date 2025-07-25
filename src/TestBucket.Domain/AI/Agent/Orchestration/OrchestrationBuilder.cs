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
using Microsoft.SemanticKernel.ChatCompletion;

using TestBucket.Domain.AI.Agent.Agents;
using TestBucket.Domain.AI.Agent.Orchestration.Strategies;
using TestBucket.Domain.AI.Agent.Orchestration.Strategies.Issues;
using TestBucket.Domain.AI.Agent.Orchestration.Strategies.Requirements;
using TestBucket.Domain.AI.Agent.Orchestration.Strategies.Testing;

namespace TestBucket.Domain.AI.Agent.Orchestration;
internal class OrchestrationBuilder
{
    private readonly IReadOnlyList<IOrchestrationStrategy> _strategies = [
            new DefaultStrategy(),
            new AddTestsStrategy(),
            new DraftTestsStrategy(),
            new AddRequirements(),
            new DraftRequirements(),
            new AIRunner(),
            new FindSimilarIssues(),
            new SummarizeIssue()
        ];

    public AgentOrchestration<string,string> Build(
        string id,
        Kernel kernel, 
        ChatHistory history,
        IServiceProvider serviceProvider,
        OrchestrationResponseCallback responseCallback,
        OrchestrationStreamingCallback streamingCallback)
    {
        IOrchestrationStrategy? strategy = _strategies.Where(x => x.Name == id).FirstOrDefault();
        if(strategy is null)
        {
            throw new ArgumentException("No strategy found with the given id", nameof(id));
        }

        return strategy.CreateOrchestration(history, kernel, serviceProvider, responseCallback, streamingCallback);
    }
}
