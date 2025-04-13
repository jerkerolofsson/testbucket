using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Automation;

namespace TestBucket.Contracts.Integrations
{
    /// <summary>
    /// Implemented by an extension
    /// </summary>
    public interface IExternalPipelineRunner
    {
        string SystemName { get; }

        /// <summary>
        /// Downloads all artifacts matching the pattern as a zip file
        /// The returned zip may contain additional files.
        /// </summary>
        /// <param name="system"></param>
        /// <param name="pipelineId"></param>
        /// <param name="jobId"></param>
        /// <param name="testResultsArtifactsPattern"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<byte[]> GetArtifactsZipAsByteArrayAsync(ExternalSystemDto system, string pipelineId, string jobId, string testResultsArtifactsPattern, CancellationToken cancellationToken);

        /// <summary>
        /// Creates a pipeline and, if successful, sets the pipeline identifier (this is unique to the external system used) as
        /// the context.CiCdPipelineIdentifier property
        /// </summary>
        /// <param name="system"></param>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task CreateAsync(ExternalSystemDto system, TestExecutionContext context, CancellationToken cancellationToken);

        /// <summary>
        /// Reads the latest status for a pipeline, including jobs, and returns it
        /// </summary>
        /// <param name="system"></param>
        /// <param name="pipelineId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<PipelineDto?> GetPipelineAsync(ExternalSystemDto system, string pipelineId, CancellationToken cancellationToken);

        /// <summary>
        /// Reads the log file from a job
        /// </summary>
        /// <param name="system"></param>
        /// <param name="jobId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        Task<string> ReadTraceAsync(ExternalSystemDto system, string jobId, CancellationToken cancellationToken);
    }
}
