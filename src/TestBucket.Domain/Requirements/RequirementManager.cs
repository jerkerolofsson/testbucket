using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Requirements.Specifications;
using TestBucket.Domain.Requirements.Specifications.Folders;
using TestBucket.Domain.Requirements.Specifications.Links;
using TestBucket.Domain.Shared;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Requirements
{
    public class RequirementManager : IRequirementManager
    {
        private readonly IRequirementRepository _repository;

        public RequirementManager(IRequirementRepository repository)
        {
            _repository = repository;
        }
        public async Task<RequirementSpecification?> GetRequirementSpecificationByIdAsync(ClaimsPrincipal principal, long id)
        {
            FilterSpecification<RequirementSpecification>[] filters = [
                new FilterByTenant<RequirementSpecification>(principal.GetTenantIdOrThrow()), 
                new FilterRequirementSpecificationById(id)
                ];

            var result = await _repository.SearchRequirementSpecificationsAsync(filters, 0, 1);
            return result.Items.FirstOrDefault();
        }

        public async Task<Requirement?> GetRequirementByIdAsync(ClaimsPrincipal principal, long id)
        {
            FilterSpecification<Requirement>[] filters = [new FilterByTenant<Requirement>(principal.GetTenantIdOrThrow()),new FilterRequirementById(id)];

            var result = await _repository.SearchRequirementsAsync(filters, 0, 1);
            return result.Items.FirstOrDefault();
        }

        public async Task<RequirementTestLink[]> GetLinksForTestAsync(ClaimsPrincipal principal, TestCase test)
        {
            FilterSpecification<RequirementTestLink>[] filters = [new FilterRequirementTestLinkByTest(test.Id)];
            return await SearchRequirementLinksAsync(principal, filters);
        }
        public async Task<RequirementTestLink[]> GetLinksForRequirementAsync(ClaimsPrincipal principal, Requirement requirement)
        {
            FilterSpecification<RequirementTestLink>[] filters = [new FilterRequirementTestLinkByRequirement(requirement.Id)];
            return await SearchRequirementLinksAsync(principal, filters);
        }

        public async Task<RequirementTestLink[]> SearchRequirementLinksAsync(ClaimsPrincipal principal, FilterSpecification<RequirementTestLink>[] filterSpecifications)
        {
            FilterSpecification<RequirementTestLink>[] filters = [new FilterByTenant<RequirementTestLink>(principal.GetTenantIdOrThrow()), .. filterSpecifications];
            return await _repository.SearchRequirementLinksAsync(filters);
        }


        public async Task DeleteRequirementLinkAsync(ClaimsPrincipal principal, RequirementTestLink requirementLink)
        {
            principal.ThrowIfNotAdmin();
            principal.ThrowIfEntityTenantIsDifferent(requirementLink);

            await _repository.DeleteRequirementLinkAsync(requirementLink);
        }

        /// <summary>
        /// Returns all requirement links for a specification
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="specification"></param>
        /// <returns></returns>
        public async Task<List<RequirementTestLink>> GetRequirementLinksForSpecificationAsync(ClaimsPrincipal principal, RequirementSpecification specification)
        {
            principal.ThrowIfEntityTenantIsDifferent(specification);
            var tenantId = principal.GetTenantIdOrThrow();

            return await _repository.GetRequirementLinksForSpecificationAsync(tenantId, specification.Id);
        }

        public async Task AddRequirementLinkAsync(ClaimsPrincipal principal, Requirement requirement, TestCase testCase)
        {
            principal.ThrowIfNotAdmin();

            var tenantId = principal.GetTenantIdOrThrow();
            var requirementLink = new RequirementTestLink 
            { 
                RequirementId = requirement.Id, 
                RequirementSpecificationId = requirement.RequirementSpecificationId,
                RequirementExternalId = requirement.ExternalId,
                TestCaseId = testCase.Id, 
                TenantId = tenantId 
            };
            await _repository.AddRequirementLinkAsync(requirementLink);
        }

        public async Task AddRequirementLinkAsync(ClaimsPrincipal principal, RequirementTestLink requirementLink)
        {
            principal.ThrowIfEntityTenantIsDifferent(requirementLink);
            await _repository.AddRequirementLinkAsync(requirementLink);
        }

        public async Task AddRequirementAsync(ClaimsPrincipal principal, Requirement requirement)
        {
            requirement.TenantId = principal.GetTenantIdOrThrow();
            requirement.Created = DateTimeOffset.UtcNow;
            requirement.Modified = DateTimeOffset.UtcNow;
            requirement.CreatedBy = principal.Identity?.Name;
            requirement.ModifiedBy = principal.Identity?.Name;

            await GenerateFoldersFromPathAsync(requirement);

            await _repository.AddRequirementAsync(requirement);
        }

        public async Task<RequirementSpecificationFolder[]> SearchRequirementFoldersAsync(ClaimsPrincipal principal, SearchRequirementQuery query)
        {
            var tenantId = principal.GetTenantIdOrThrow();

            List<FilterSpecification<RequirementSpecificationFolder>> filters = [ new FilterByTenant<RequirementSpecificationFolder>(tenantId) ];
            if(query.CompareFolder)
            {
                filters.Add(new FilterRequirementFoldersByParentId(query.FolderId));
            }
            if (query.RequirementSpecificationId is not null)
            {
                filters.Add(new FilterRequirementFoldersBySpecification(query.RequirementSpecificationId));
            }

            return await _repository.SearchRequirementFoldersAsync(filters.ToArray());
        }

        private async Task GenerateFoldersFromPathAsync(Requirement requirement)
        {
            if (requirement.Path is not null)
            {
                long? parentId = null;
                foreach (var folderName in requirement.Path.Split('/', StringSplitOptions.TrimEntries|StringSplitOptions.RemoveEmptyEntries))
                {
                    FilterSpecification<RequirementSpecificationFolder>[] filters = [
                        new FilterByTenant<RequirementSpecificationFolder>(requirement.TenantId!),
                        new FilterRequirementFoldersBySpecification(requirement.RequirementSpecificationId),
                        new FilterRequirementFoldersByParentId(parentId),
                        new FilterRequirementFoldersByName(folderName),
                        ];

                    var folder = (await _repository.SearchRequirementFoldersAsync(filters)).FirstOrDefault();
                    if (folder is null)
                    {
                        folder = new RequirementSpecificationFolder
                        {
                            Name = folderName,
                            TestProjectId = requirement.TestProjectId,
                            TenantId = requirement.TenantId,
                            RequirementSpecificationId = requirement.RequirementSpecificationId,
                            ParentId = parentId
                        };
                        await _repository.AddRequirementFolderAsync(folder);
                    }

                    parentId = folder.Id;
                    requirement.RequirementSpecificationFolderId = folder.Id;
                }
            }
        }

        public async Task DeleteRequirementAsync(ClaimsPrincipal principal, Requirement requirement)
        {
            principal.ThrowIfEntityTenantIsDifferent(requirement);
            await _repository.DeleteRequirementAsync(requirement);
        }

        /// <summary>
        /// Updates a requirement
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        public Task UpdateRequirementAsync(ClaimsPrincipal principal, Requirement requirement)
        {
            principal.ThrowIfNotAdmin();
            principal.ThrowIfEntityTenantIsDifferent(requirement);

            requirement.ModifiedBy = principal.Identity?.Name;
            requirement.Modified = DateTimeOffset.UtcNow;
            return _repository.UpdateRequirementAsync(requirement);
        }

        /// <summary>
        /// Adds a new requirement specification
        /// </summary>
        /// <param name="principal">User making changes</param>
        /// <param name="specification">Entity to save</param>
        /// <returns></returns>
        public Task AddRequirementSpecificationAsync(ClaimsPrincipal principal, RequirementSpecification specification)
        {
            specification.TenantId = principal.GetTenantIdOrThrow();
            specification.Created = DateTimeOffset.UtcNow;
            specification.Modified = DateTimeOffset.UtcNow;
            specification.CreatedBy = principal.Identity?.Name;
            specification.ModifiedBy = principal.Identity?.Name;
            return _repository.AddRequirementSpecificationAsync(specification);
        }


        public async Task DeleteRequirementSpecificationAsync(ClaimsPrincipal principal, RequirementSpecification specification)
        {
            principal.ThrowIfNotAdmin();
            principal.ThrowIfEntityTenantIsDifferent(specification);

            await _repository.DeleteRequirementSpecificationAsync(specification);
        }

        public async Task DeleteSpecificationRequirementsAndFoldersAsync(ClaimsPrincipal principal, RequirementSpecification specification)
        {
            principal.ThrowIfEntityTenantIsDifferent(specification);
            await _repository.DeleteSpecificationRequirementsAndFoldersAsync(specification);
        }

        public async Task<PagedResult<Requirement>> SearchRequirementsAsync(ClaimsPrincipal principal, SearchRequirementQuery query)
        {
            var filters = Specifications.RequirementSpecificationBuilder.From(query);
            filters = [.. filters, new FilterByTenant<Requirement>(principal.GetTenantIdOrThrow())];

            return await _repository.SearchRequirementsAsync(filters, query.Offset, query.Count);
        }

        /// <summary>
        /// Searches for requirement specifications
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<PagedResult<RequirementSpecification>> SearchRequirementSpecificationsAsync(ClaimsPrincipal principal, SearchQuery query)
        {
            var filters = Specifications.RequirementSpecificationBuilder.From(query);
            filters = [.. filters, new FilterByTenant<RequirementSpecification>(principal.GetTenantIdOrThrow())];

            return await _repository.SearchRequirementSpecificationsAsync(filters, query.Offset, query.Count);
        }

        /// <summary>
        /// Updates a requirement specification
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="specification"></param>
        /// <returns></returns>
        public Task UpdateRequirementSpecificationAsync(ClaimsPrincipal principal, RequirementSpecification specification)
        {
            principal.ThrowIfEntityTenantIsDifferent(specification);
            specification.ModifiedBy = principal.Identity?.Name;
            specification.Modified = DateTimeOffset.UtcNow;
            return _repository.UpdateRequirementSpecificationAsync(specification);
        }
    }
}
