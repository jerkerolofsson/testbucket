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
    Task AddAsync(ClaimsPrincipal principal, Pipeline pipeline);
    void AddObserver(IPipelineObserver observer);
    Task<IReadOnlyList<IExternalPipelineRunner>> GetExternalPipelineRunnersAsync(ClaimsPrincipal principal, long testProjectId);
    Task<ExternalSystemDto[]> GetIntegrationConfigsAsync(ClaimsPrincipal principal, long testProjectId);
    Task<Pipeline?> GetPipelineByIdAsync(ClaimsPrincipal principal, long id);
    Task<IReadOnlyList<Pipeline>> GetPipelinesForTestRunAsync(ClaimsPrincipal principal, long id);
    Task<IReadOnlyList<ExternalSystem>> GetProjectIntegrationsAsync(ClaimsPrincipal principal, long testProjectId);
    Task<string?> ReadTraceAsync(ClaimsPrincipal principal, Pipeline pipeline, string pipelineJobIdentifier, CancellationToken cancellationToken);
    void RemoveObserver(IPipelineObserver observer);
}
