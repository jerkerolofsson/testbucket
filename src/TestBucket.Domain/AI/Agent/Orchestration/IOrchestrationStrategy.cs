using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Orchestration;
using Microsoft.SemanticKernel.ChatCompletion;

namespace TestBucket.Domain.AI.Agent.Orchestration;
public interface IOrchestrationStrategy
{
    public string Name { get; }

    AgentOrchestration<string, string> CreateOrchestration(ChatHistory history, Kernel kernel, IServiceProvider serviceProvider, OrchestrationResponseCallback responseCallback, OrchestrationStreamingCallback streamingCallback);
}
