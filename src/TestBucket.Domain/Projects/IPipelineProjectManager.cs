using System.Security.Claims;

using TestBucket.Contracts.Integrations;
using TestBucket.Contracts.Projects;

namespace TestBucket.Domain.Projects;
public interface IPipelineProjectManager
{
    Task CreatePipelineAsync(ClaimsPrincipal principal, TestExecutionContext context);
}