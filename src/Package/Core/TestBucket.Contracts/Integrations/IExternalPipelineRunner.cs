using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Automation;
using TestBucket.Contracts.Projects;

namespace TestBucket.Contracts.Integrations
{
    public interface IExternalPipelineRunner
    {
        string SystemName { get; }

        /// <summary>
        /// Creates a pipeline and, if successful, sets the pipeline identifier (this is unique to the external system used) as
        /// the context.CiCdPipelineIdentifier property
        /// </summary>
        /// <param name="system"></param>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task CreateAsync(ExternalSystemDto system, TestExecutionContext context, CancellationToken cancellationToken);
        Task<PipelineDto?> GetPipelineAsync(ExternalSystemDto system, string pipelineId, CancellationToken cancellationToken);
        Task<string> ReadTraceAsync(ExternalSystemDto system, string jobId, CancellationToken cancellationToken);
    }
}
