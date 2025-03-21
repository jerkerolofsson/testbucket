using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Requirements
{
    public interface IRequirementRepository
    {
        /// <summary>
        /// Adds a requirement
        /// </summary>
        /// <param name="requirement"></param>
        /// <returns></returns>
        Task AddRequirementAsync(Requirement requirement);

        /// <summary>
        /// Deletes the requirement
        /// </summary>
        /// <param name="requirement"></param>
        /// <returns></returns>
        Task DeleteRequirementAsync(Requirement requirement);

        /// <summary>
        /// Adds a new requirement specification
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="spec"></param>
        /// <returns></returns>
        Task AddRequirementSpecificationAsync(RequirementSpecification spec);

        /// <summary>
        /// Deletes a requirement specification, all folders and all requirements
        /// </summary>
        /// <param name="specification"></param>
        /// <returns></returns>
        Task DeleteRequirementSpecificationAsync(RequirementSpecification specification);

        /// <summary>
        /// Deletes all requirements and folders in the specification
        /// </summary>
        /// <param name="specification"></param>
        /// <returns></returns>
        Task DeleteSpecificationRequirementsAndFoldersAsync(RequirementSpecification specification);

        /// <summary>
        /// Searches for requirements
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        Task<PagedResult<Requirement>> SearchRequirementsAsync(IEnumerable<FilterSpecification<Requirement>> filters, int offset, int count);

        /// <summary>
        /// Searches for requirement specifications
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        Task<PagedResult<RequirementSpecification>> SearchRequirementSpecificationsAsync(IEnumerable<FilterSpecification<RequirementSpecification>> filters, int offset, int count);

        /// <summary>
        /// Saves changes to a requirement
        /// </summary>
        /// <param name="requirement"></param>
        /// <returns></returns>
        Task UpdateRequirementAsync(Requirement requirement);

        /// <summary>
        /// Updates a requirement specification
        /// </summary>
        /// <param name="specification"></param>
        /// <returns></returns>
        Task UpdateRequirementSpecificationAsync(RequirementSpecification specification);

        /// <summary>
        /// Deletes a requirement link
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        Task DeleteRequirementLinkAsync(RequirementTestLink link);

        /// <summary>
        /// Adds a requirement link
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        Task AddRequirementLinkAsync(RequirementTestLink link);

        /// <summary>
        /// Searches for requirement links
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        Task<RequirementTestLink[]> SearchRequirementLinksAsync(FilterSpecification<RequirementTestLink>[] filters);
    }
}
