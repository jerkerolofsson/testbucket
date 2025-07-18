﻿using System;
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
        /// Returns the requirement, or null if not found
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Requirement?> GetRequirementByIdAsync(string tenantId, long id);

        /// <summary>
        /// Adds a new requirement specification
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="spec"></param>
        /// <returns></returns>
        Task AddRequirementSpecificationAsync(RequirementSpecification spec);

        /// <summary>
        /// Returns a requirement specification from an ID
        /// </summary>
        /// <param name="requirementSpecificationId"></param>
        /// <returns></returns>
        Task<RequirementSpecification?> GetRequirementSpecificationByIdAsync(string tenantId, long requirementSpecificationId);

        /// <summary>
        /// Returns all IDs matching the filters
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        Task<long[]> SearchRequirementIdsAsync(FilterSpecification<Requirement>[] filters);

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

        Task<PagedResult<Requirement>> SemanticSearchRequirementsAsync(ReadOnlyMemory<float> value, IReadOnlyList<FilterSpecification<Requirement>> filters, int offset, int count);

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

        /// <summary>
        /// Returns all requirement links for a specification
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="requirementSpecificationId"></param>
        /// <returns></returns>
        Task<List<RequirementTestLink>> GetRequirementLinksForSpecificationAsync(string tenantId, long requirementSpecificationId);

        #region Folders
        /// <summary>
        /// Searches for folders
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        Task<RequirementSpecificationFolder[]> SearchRequirementFoldersAsync(FilterSpecification<RequirementSpecificationFolder>[] filters);

        /// <summary>
        /// Returns a folder by ID
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="folderId"></param>
        /// <returns></returns>
        Task<RequirementSpecificationFolder?> GetRequirementFolderByIdAsync(string tenantId, long folderId);

        /// <summary>
        /// Adds a folder
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        Task AddRequirementFolderAsync(RequirementSpecificationFolder folder);

        /// <summary>
        /// Updates the folder
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        Task UpdateRequirementFolderAsync(RequirementSpecificationFolder folder);

        /// <summary>
        /// Deletes the folder
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        Task DeleteRequirementFolderAsync(RequirementSpecificationFolder folder);
        #endregion Folders
    }
}
