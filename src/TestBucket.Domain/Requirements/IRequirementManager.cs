using System.Security.Claims;

using TestBucket.Domain.Requirements.Models;

namespace TestBucket.Domain.Requirements;
public interface IRequirementManager
{
    Task AddRequirementSpecificationAsync(ClaimsPrincipal principal, RequirementSpecification specification);
    Task<PagedResult<RequirementSpecification>> SearchRequirementSpecificationsAsync(ClaimsPrincipal principal, SearchQuery query);
    Task UpdateRequirementSpecificationAsync(ClaimsPrincipal principal, RequirementSpecification specification);
}