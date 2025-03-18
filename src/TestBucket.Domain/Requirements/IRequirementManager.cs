using System.Security.Claims;

using TestBucket.Domain.Requirements.Models;

namespace TestBucket.Domain.Requirements;
public interface IRequirementManager
{
    #region Requirements
    /// <summary>
    /// Adds a new requirement 
    /// </summary>
    /// <param name="principal">User making changes</param>
    /// <param name="requirement">Entity to save</param>
    /// <returns></returns>
    Task AddRequirementAsync(ClaimsPrincipal principal, Requirement requirement);

    /// <summary>
    /// Updates a requirement
    /// </summary>
    /// <param name="principal">User making changes</param>
    /// <param name="requirement">Entity to save</param>
    /// <returns></returns>
    Task UpdateRequirementAsync(ClaimsPrincipal principal, Requirement requirement);
    Task<PagedResult<Requirement>> SearchRequirementsAsync(ClaimsPrincipal principal, SearchRequirementQuery query);

    #endregion Requirements

    #region Requirement Specifications

    /// <summary>
    /// Adds a new requirement spec.
    /// </summary>
    /// <param name="principal">User making changes</param>
    /// <param name="specification">Entity to save</param>
    /// <returns></returns>
    Task AddRequirementSpecificationAsync(ClaimsPrincipal principal, RequirementSpecification specification);
    Task DeleteRequirementSpecificationAsync(ClaimsPrincipal principal, RequirementSpecification specification);
    Task DeleteSpecificationRequirementsAndFoldersAsync(ClaimsPrincipal principal, RequirementSpecification specification);

    /// <summary>
    /// Searches for requirement specifications
    /// </summary>
    /// <param name="principal">User searching</param>
    /// <param name="query"></param>
    /// <returns></returns>
    Task<PagedResult<RequirementSpecification>> SearchRequirementSpecificationsAsync(ClaimsPrincipal principal, SearchQuery query);

    /// <summary>
    /// Updates a requirement specification
    /// </summary>
    /// <param name="principal">User making changes</param>
    /// <param name="specification"></param>
    /// <returns></returns>
    Task UpdateRequirementSpecificationAsync(ClaimsPrincipal principal, RequirementSpecification specification);

    #endregion Requirement Specifications
}