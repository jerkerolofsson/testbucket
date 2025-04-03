using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Projects;

namespace TestBucket.Contracts.Integrations
{
    public interface IExternalPipelineRunner
    {
        string SystemName { get; }

        Task StartAsync(ExternalSystemDto system, TestExecutionContext context, CancellationToken cancellationToken);
    }
}
