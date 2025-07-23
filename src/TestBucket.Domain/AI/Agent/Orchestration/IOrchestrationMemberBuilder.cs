using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

namespace TestBucket.Domain.AI.Agent.Orchestration;
public interface IOrchestrationMemberBuilder
{
    ChatCompletionAgent[] GetMembers(Kernel kernel, ILoggerFactory loggerFactory);
}
