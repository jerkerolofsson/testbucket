using System.Security.Claims;

using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

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

    /// <summary>
    /// Searches for requirements
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    Task<PagedResult<Requirement>> SearchRequirementsAsync(ClaimsPrincipal principal, SearchRequirementQuery query);

    /// <summary>
    /// Searches for requirements
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    Task<PagedResult<Requirement>> SearchRequirementsAsync(ClaimsPrincipal principal, FilterSpecification<Requirement>[] specifications, int offset, int count);

    /// <summary>
    /// Deletes the specified requirement
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="requirement"></param>
    /// <returns></returns>
    Task DeleteRequirementAsync(ClaimsPrincipal principal, Requirement requirement);
    Task<Requirement?> GetRequirementByIdAsync(ClaimsPrincipal principal, long id);

    #endregion Requirements

    #region Folders
    Task AddFolderAsync(ClaimsPrincipal principal, RequirementSpecificationFolder folder);
    Task UpdateFolderAsync(ClaimsPrincipal principal, RequirementSpecificationFolder folder);
    Task DeleteFolderAsync(ClaimsPrincipal principal, RequirementSpecificationFolder folder);

    /// <summary>
    /// Searches for requirements
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    Task<RequirementSpecificationFolder[]> SearchRequirementFoldersAsync(ClaimsPrincipal principal, SearchRequirementFolderQuery query);

    #endregion

    #region Observer

    /// <summary>
    /// Adds an observer
    /// </summary>
    /// <param name="listener"></param>
    public void AddObserver(IRequirementObserver observer);

    /// <summary>
    /// Removes an observer
    /// </summary>
    /// <param name="observer"></param>
    public void RemoveObserver(IRequirementObserver observer);

    #endregion Observer

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
    /// Searches for requirement specifications
    /// </summary>
    /// <param name="filters"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    Task<PagedResult<RequirementSpecification>> SearchRequirementSpecificationsAsync(ClaimsPrincipal principal, FilterSpecification<RequirementSpecification>[] filters, int offset, int count);

    /// <summary>
    /// Updates a requirement specification
    /// </summary>
    /// <param name="principal">User making changes</param>
    /// <param name="specification"></param>
    /// <returns></returns>
    Task UpdateRequirementSpecificationAsync(ClaimsPrincipal principal, RequirementSpecification specification);
    Task<RequirementSpecification?> GetRequirementSpecificationByIdAsync(ClaimsPrincipal principal, long id);

    #endregion Requirement Specifications

    #region Requirement Links
    Task<RequirementTestLink[]> GetLinksForRequirementAsync(ClaimsPrincipal principal, Requirement requirement);
    Task<RequirementTestLink[]> GetLinksForTestAsync(ClaimsPrincipal principal, TestCase test);
    Task AddRequirementLinkAsync(ClaimsPrincipal principal, Requirement requirement, TestCase testCase);
    Task AddRequirementLinkAsync(ClaimsPrincipal principal, RequirementTestLink link);
    Task DeleteRequirementLinkAsync(ClaimsPrincipal principal, RequirementTestLink link);
    Task<List<RequirementTestLink>> GetRequirementLinksForSpecificationAsync(ClaimsPrincipal principal, RequirementSpecification specification);
    Task GenerateFoldersFromPathAsync(Requirement requirement);
    #endregion
}