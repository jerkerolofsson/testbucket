using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Integrations;
using TestBucket.Contracts.Projects;
using TestBucket.Domain.Automation.Models;

namespace TestBucket.Domain.Automation.Services;
public interface IPipelineManager
{
    Task AddAsync(ClaimsPrincipal principal, Pipeline pipeline);
    Task<IReadOnlyList<IExternalPipelineRunner>> GetExternalPipelineRunnersAsync(ClaimsPrincipal principal, long testProjectId);
    Task<ExternalSystemDto[]> GetIntegrationConfigsAsync(ClaimsPrincipal principal, long testProjectId);
    Task<Pipeline?> GetPipelineByIdAsync(ClaimsPrincipal principal, long id);
    Task<IReadOnlyList<Pipeline>> GetPipelinesForTestRunAsync(ClaimsPrincipal principal, long id);
    Task<IReadOnlyList<ExternalSystem>> GetProjectIntegrationsAsync(ClaimsPrincipal principal, long testProjectId);
}
