using System.Security.Claims;

using TestBucket.Domain.Requirements.Models;

namespace TestBucket.Domain.Requirements;
public interface IRequirementManager
{
    Task AddRequirementAsync(ClaimsPrincipal principal, Requirement requirement);
    Task AddRequirementSpecificationAsync(ClaimsPrincipal principal, RequirementSpecification specification);
    Task<PagedResult<Requirement>> SearchRequirementsAsync(ClaimsPrincipal principal, SearchRequirementQuery query);
    Task<PagedResult<RequirementSpecification>> SearchRequirementSpecificationsAsync(ClaimsPrincipal principal, SearchQuery query);
    Task UpdateRequirementSpecificationAsync(ClaimsPrincipal principal, RequirementSpecification specification);
}