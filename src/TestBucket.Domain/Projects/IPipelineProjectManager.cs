using System.Security.Claims;

using TestBucket.Contracts.Integrations;

namespace TestBucket.Domain.Projects;
public interface IPipelineProjectManager
{
    Task CreatePipelineAsync(ClaimsPrincipal principal, TestExecutionContext context);
    Task<IReadOnlyList<IExternalPipelineRunner>> GetExternalPipelineRunnersAsync(ClaimsPrincipal principal, long testProjectId);
}