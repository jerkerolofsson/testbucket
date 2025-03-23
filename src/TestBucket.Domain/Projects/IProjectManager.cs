using System.Security.Claims;

using TestBucket.Traits.Core;

namespace TestBucket.Domain.Projects;
public interface IProjectManager
{
    Task AddAsync(ClaimsPrincipal principal, TestProject project);
    Task<string[]?> GetFieldOptionsAsync(ClaimsPrincipal principal, long testProjectId, TraitType traitType, CancellationToken cancellationToken);
    Task<IReadOnlyList<ExternalSystem>> GetProjectIntegrationsAsync(ClaimsPrincipal principal, string slug);
    Task SaveProjectIntegrationsAsync(ClaimsPrincipal principal, string slug, ExternalSystem system);
}