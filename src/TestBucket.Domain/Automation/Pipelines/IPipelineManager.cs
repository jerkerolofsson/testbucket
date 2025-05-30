using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Integrations;
using TestBucket.Domain.Automation.Pipelines.Models;

namespace TestBucket.Domain.Automation.Pipelines;
public interface IPipelineManager
{
    /// <summary>
    /// Adds a pipeline. This will start a monitoring thread for the pipeline
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="pipeline"></param>
    /// <returns></returns>
    Task AddAsync(ClaimsPrincipal principal, Pipeline pipeline);

    /// <summary>
    /// Returns available runners
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="testProjectId"></param>
    /// <returns></returns>
    Task<IReadOnlyList<IExternalPipelineRunner>> GetExternalPipelineRunnersAsync(ClaimsPrincipal principal, long testProjectId);

    /// <summary>
    /// Gets all configurations as DTOs (this is a DTO mapping of GetIntegrationConfigsAsync)
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="testProjectId"></param>
    /// <returns></returns>
    Task<ExternalSystemDto[]> GetIntegrationConfigsAsync(ClaimsPrincipal principal, long testProjectId);

    /// <summary>
    /// Gets a pipeline by id
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<Pipeline?> GetPipelineByIdAsync(ClaimsPrincipal principal, long id);

    /// <summary>
    /// Gets a pipeline by the components that define the external system
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="ciCdSystemName"></param>
    /// <param name="ciCdProjectId"></param>
    /// <param name="ciCdPipelineId"></param>
    /// <returns></returns>
    Task<Pipeline?> GetPipelineByExternalAsync(ClaimsPrincipal principal, string ciCdSystemName, string ciCdProjectId, string ciCdPipelineId);

    /// <summary>
    /// Returns all pipelines for a run
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<IReadOnlyList<Pipeline>> GetPipelinesForTestRunAsync(ClaimsPrincipal principal, long id);

    /// <summary>
    /// Returns all integrations
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="testProjectId"></param>
    /// <returns></returns>
    Task<IReadOnlyList<ExternalSystem>> GetProjectIntegrationsAsync(ClaimsPrincipal principal, long testProjectId);

    /// <summary>
    /// Reads a trace from the pipeline (job log)
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="pipeline"></param>
    /// <param name="pipelineJobIdentifier"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string?> ReadTraceAsync(ClaimsPrincipal principal, Pipeline pipeline, string pipelineJobIdentifier, CancellationToken cancellationToken);

    /// <summary>
    /// Adds an observer
    /// </summary>
    /// <param name="listener"></param>
    void AddObserver(IPipelineObserver observer);

    /// <summary>
    /// Removes an observer
    /// </summary>
    /// <param name="observer"></param>
    void RemoveObserver(IPipelineObserver observer);
    Task StartMonitorIfNotMonitoringAsync(ClaimsPrincipal user, Pipeline existingPipeline);
}
