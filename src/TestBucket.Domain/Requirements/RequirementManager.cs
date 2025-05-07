using System.Collections.Generic;

using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Requirements.Specifications;
using TestBucket.Domain.Requirements.Specifications.Folders;
using TestBucket.Domain.Requirements.Specifications.Links;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

using TestBucket.Domain.Requirements.Specifications.Requirements;
using TestBucket.Domain.Requirements.Events;
using Mediator;
using TestBucket.Domain.Traceability.Models;
using TestBucket.Domain.Traceability;

namespace TestBucket.Domain.Requirements
{
    public class RequirementManager : IRequirementManager
    {
        private readonly IRequirementRepository _repository;
        private readonly List<IRequirementObserver> _requirementObservers = new();
        private readonly IMediator _mediator;
        public RequirementManager(IRequirementRepository repository, IMediator mediator)
        {
            _repository = repository;
            _mediator = mediator;
        }

        #region Observer

        /// <summary>
        /// Adds an observer
        /// </summary>
        /// <param name="listener"></param>
        public void AddObserver(IRequirementObserver observer) => _requirementObservers.Add(observer);

        /// <summary>
        /// Removes an observer
        /// </summary>
        /// <param name="observer"></param>
        public void RemoveObserver(IRequirementObserver observer) => _requirementObservers.Remove(observer);

        #endregion Observer

        #region Requirement

        private async Task<IReadOnlyList<Requirement>> GetChildRequirementsAsync(ClaimsPrincipal principal, long rootId, bool recurse)
        {
            var result = new List<Requirement>();

            // If this is a root requirement, we can quickly find all downstream requirements as they all refer to us as the root
            var specifications = new List<FilterSpecification<Requirement>>()
            {
                new FilterByTenant<Requirement>(principal.GetTenantIdOrThrow()),
                new FilterRequirementByParentId(rootId)
            };
            int offset = 0;
            int pageSize = 250;
            while (true)
            {
                var searchResult = await _repository.SearchRequirementsAsync(specifications, 0, pageSize);
                result.AddRange(searchResult.Items);
                if (recurse)
                {
                    foreach(var child in searchResult.Items)
                    {
                        await GetChildRequirementsAsync(principal, child.Id, recurse: true);
                    }
                }

                if (searchResult.Items.Length != pageSize)
                {
                    break;
                }
                offset += pageSize;
            }

            return result;
        }
        private async Task<IReadOnlyList<Requirement>> GetRequirementsByRootAsync(ClaimsPrincipal principal, long rootId)
        {
            var result = new List<Requirement>();

            // If this is a root requirement, we can quickly find all downstream requirements as they all refer to us as the root
            var specifications = new List<FilterSpecification<Requirement>>()
            {
                new FilterByTenant<Requirement>(principal.GetTenantIdOrThrow()),
                new FilterRequirementByRootId(rootId)
            };
            int offset = 0;
            int pageSize = 250;
            while (true)
            {
                var searchResult = await _repository.SearchRequirementsAsync(specifications, 0, pageSize);
                result.AddRange(searchResult.Items);

                if (searchResult.Items.Length != pageSize)
                {
                    break;
                }
                offset += pageSize;
            }

            return result;
        }

        public async Task<IReadOnlyList<Requirement>> GetDownstreamRequirementsAsync(ClaimsPrincipal principal, Requirement requirement)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.Requirement, PermissionLevel.Read);
            principal.ThrowIfEntityTenantIsDifferent(requirement);

            var result = new List<Requirement>();
            result.AddRange(await GetRequirementsByRootAsync(principal, requirement.Id));

            if(result.Count == 0)
            {
                // There are no requirements that point to 'requirement' as root, so this may be an intermediate requirement.
                // Search for all descendants
                result.AddRange(await GetChildRequirementsAsync(principal, requirement.Id, true));
            }

            return result;
        }

        public async Task DeleteRequirementAsync(ClaimsPrincipal principal, Requirement requirement)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.Requirement, PermissionLevel.Delete);
            principal.ThrowIfEntityTenantIsDifferent(requirement);
            await _repository.DeleteRequirementAsync(requirement);

            foreach (var observer in _requirementObservers)
            {
                await observer.OnRequirementDeletedAsync(requirement);
            }

            await _mediator.Publish(new RequirementDeletedEvent(principal,requirement));
        }

        /// <summary>
        /// Updates a requirement
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        public async Task UpdateRequirementAsync(ClaimsPrincipal principal, Requirement requirement)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.Requirement, PermissionLevel.Write);
            string tenantId = principal.ThrowIfEntityTenantIsDifferent(requirement);

            await GenerateFoldersFromPathAsync(requirement);

            requirement.ModifiedBy = principal.Identity?.Name;
            requirement.Modified = DateTimeOffset.UtcNow;
            await _repository.UpdateRequirementAsync(requirement);
            foreach (var observer in _requirementObservers)
            {
                await observer.OnRequirementSavedAsync(requirement);
            }

            await _mediator.Publish(new RequirementUpdatedEvent(principal, requirement));
        }


        public async Task<IReadOnlyList<Requirement>> GetRequirementsByAncestorFolderIdAsync(ClaimsPrincipal principal, long folderId, int offset, int count)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.Requirement, PermissionLevel.Read);

            FilterSpecification<Requirement>[] filters = [
                new FilterByTenant<Requirement>(principal.GetTenantIdOrThrow()),
                new FilterRequirementByAncestorFolder(folderId)
                ];

            var result = await _repository.SearchRequirementsAsync(filters, offset, count);
            return result.Items;
        }


        /// <summary>
        /// Searches for requirements
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<PagedResult<Requirement>> SearchRequirementsAsync(ClaimsPrincipal principal, SearchRequirementQuery query)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.Requirement, PermissionLevel.Read);

            var filters = RequirementSpecificationBuilder.From(query);
            filters = [.. filters, new FilterByTenant<Requirement>(principal.GetTenantIdOrThrow())];

            return await _repository.SearchRequirementsAsync(filters, query.Offset, query.Count);
        }

        /// <summary>
        /// Searches for requirements
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="specifications"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public async Task<PagedResult<Requirement>> SearchRequirementsAsync(ClaimsPrincipal principal, FilterSpecification<Requirement>[] specifications, int offset, int count)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.Requirement, PermissionLevel.Read);
            return await _repository.SearchRequirementsAsync(specifications, offset, count);
        }

        /// <summary>
        /// Gets a requirement by id
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Requirement?> GetRequirementByIdAsync(ClaimsPrincipal principal, long id)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.Requirement, PermissionLevel.Read);

            FilterSpecification<Requirement>[] filters = [new FilterByTenant<Requirement>(principal.GetTenantIdOrThrow()), new FilterRequirementById(id)];

            var result = await _repository.SearchRequirementsAsync(filters, 0, 1);
            return result.Items.FirstOrDefault();
        }

        /// <summary>
        /// Gets a requirement by slug
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="slug"></param>
        /// <returns></returns>
        public async Task<Requirement?> GetRequirementBySlugAsync(ClaimsPrincipal principal, string slug)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.Requirement, PermissionLevel.Read);

            FilterSpecification<Requirement>[] filters = [
                new FilterByTenant<Requirement>(principal.GetTenantIdOrThrow()),
                new FilterRequirementBySlug(slug)];

            var result = await _repository.SearchRequirementsAsync(filters, 0, 1);
            return result.Items.FirstOrDefault();
        }


        public async Task AddRequirementAsync(ClaimsPrincipal principal, Requirement requirement)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.Requirement, PermissionLevel.Write);

            requirement.TenantId = principal.GetTenantIdOrThrow();
            requirement.Created = DateTimeOffset.UtcNow;
            requirement.Modified = DateTimeOffset.UtcNow;
            requirement.CreatedBy = principal.Identity?.Name;
            requirement.ModifiedBy = principal.Identity?.Name;

            await GenerateFoldersFromPathAsync(requirement);

            await _repository.AddRequirementAsync(requirement);

            foreach (var observer in _requirementObservers)
            {
                await observer.OnRequirementCreatedAsync(requirement);
            }

            await _mediator.Publish(new RequirementCreatedEvent(principal, requirement));
        }


        #endregion Requirement

        #region Requirement Links

        /// <summary>
        /// Returns all links for a test
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="test"></param>
        /// <returns></returns>
        public async Task<RequirementTestLink[]> GetLinksForTestAsync(ClaimsPrincipal principal, TestCase test)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.TestCase, PermissionLevel.Read);

            FilterSpecification<RequirementTestLink>[] filters = [new FilterRequirementTestLinkByTest(test.Id)];
            return await SearchRequirementLinksAsync(principal, filters);
        }

        /// <summary>
        /// Returns all links for a requirement
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        public async Task<RequirementTestLink[]> GetLinksForRequirementAsync(ClaimsPrincipal principal, Requirement requirement)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.Requirement, PermissionLevel.Read);

            FilterSpecification<RequirementTestLink>[] filters = [new FilterRequirementTestLinkByRequirement(requirement.Id)];
            return await SearchRequirementLinksAsync(principal, filters);
        }

        /// <summary>
        /// Searches for requirement links
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="filterSpecifications"></param>
        /// <returns></returns>
        public async Task<RequirementTestLink[]> SearchRequirementLinksAsync(ClaimsPrincipal principal, FilterSpecification<RequirementTestLink>[] filterSpecifications)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.Requirement, PermissionLevel.Read);

            FilterSpecification<RequirementTestLink>[] filters = [new FilterByTenant<RequirementTestLink>(principal.GetTenantIdOrThrow()), .. filterSpecifications];
            return await _repository.SearchRequirementLinksAsync(filters);
        }


        /// <summary>
        /// Removes a requirement link
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="requirementLink"></param>
        /// <returns></returns>
        public async Task DeleteRequirementLinkAsync(ClaimsPrincipal principal, RequirementTestLink requirementLink)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.Requirement, PermissionLevel.Delete);

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
            principal.ThrowIfNoPermission(PermissionEntityType.Requirement, PermissionLevel.Read);

            var tenantId = principal.GetTenantIdOrThrow();

            return await _repository.GetRequirementLinksForSpecificationAsync(tenantId, specification.Id);
        }

        public async Task AddRequirementLinkAsync(ClaimsPrincipal principal, Requirement requirement, long testCaseId)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.Requirement, PermissionLevel.Write);
            principal.ThrowIfNoPermission(PermissionEntityType.TestCase, PermissionLevel.Write);

            var tenantId = principal.GetTenantIdOrThrow();

            // Delete any existing requirement so we don't add duplicates in case
            // we are batch updating
            if(requirement.TestLinks is not null)
            {
                var linkAlreadyExists = requirement.TestLinks.Where(x => x.TestCaseId == testCaseId).Any();
                if(linkAlreadyExists)
                {
                    return;
                }
            }

            var requirementLink = new RequirementTestLink
            {
                RequirementId = requirement.Id,
                RequirementSpecificationId = requirement.RequirementSpecificationId,
                RequirementExternalId = requirement.ExternalId,
                TestCaseId = testCaseId,
                TenantId = tenantId
            };
            await _repository.AddRequirementLinkAsync(requirementLink);
        }
        public async Task AddRequirementLinkAsync(ClaimsPrincipal principal, Requirement requirement, TestCase testCase)
        {
            await AddRequirementLinkAsync(principal, requirement, testCase.Id);
        }

        public async Task AddRequirementLinkAsync(ClaimsPrincipal principal, RequirementTestLink requirementLink)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.Requirement, PermissionLevel.Write);
            principal.ThrowIfNoPermission(PermissionEntityType.TestCase, PermissionLevel.Write);
            requirementLink.TenantId = principal.GetTenantIdOrThrow();
            await _repository.AddRequirementLinkAsync(requirementLink);
        }

        #endregion Requirement Links

        #region Folders

        public async Task<RequirementSpecificationFolder?> GetRequirementFolderByIdAsync(ClaimsPrincipal principal, long id)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.RequirementSpecification, PermissionLevel.Read);

            var tenantId = principal.GetTenantIdOrThrow();
            return await _repository.GetRequirementFolderByIdAsync(tenantId, id);
        }

        public async Task AddFolderAsync(ClaimsPrincipal principal, RequirementSpecificationFolder folder)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.RequirementSpecification, PermissionLevel.Write);
            folder.TenantId = principal.GetTenantIdOrThrow();
            await _repository.AddRequirementFolderAsync(folder);

            foreach (var observer in _requirementObservers)
            {
                await observer.OnFolderCreatedAsync(folder);
            }
        }
        public async Task UpdateFolderAsync(ClaimsPrincipal principal, RequirementSpecificationFolder folder)
        {
            principal.ThrowIfEntityTenantIsDifferent(folder);
            principal.ThrowIfNoPermission(PermissionEntityType.RequirementSpecification, PermissionLevel.Write);
            await _repository.UpdateRequirementFolderAsync(folder);

            foreach (var observer in _requirementObservers)
            {
                await observer.OnFolderSavedAsync(folder);
            }
        }
        public async Task DeleteFolderAsync(ClaimsPrincipal principal, RequirementSpecificationFolder folder)
        {
            principal.ThrowIfEntityTenantIsDifferent(folder);
            principal.ThrowIfNoPermission(PermissionEntityType.RequirementSpecification, PermissionLevel.Delete);
            await _repository.DeleteRequirementFolderAsync(folder);

            foreach (var observer in _requirementObservers)
            {
                await observer.OnFolderDeletedAsync(folder);
            }
        }

        public async Task<RequirementSpecificationFolder[]> SearchRequirementFoldersAsync(ClaimsPrincipal principal, SearchRequirementFolderQuery query)
        {
            var tenantId = principal.GetTenantIdOrThrow();

            var filters = RequirementSpecificationBuilder.From(query);
            filters = [.. filters, new FilterByTenant<RequirementSpecificationFolder>(tenantId)];

            return await _repository.SearchRequirementFoldersAsync(filters.ToArray());
        }

        public async Task GenerateFoldersFromPathAsync(Requirement requirement)
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
                        // Folder does not exist, create one
                        folder = new RequirementSpecificationFolder
                        {
                            Name = folderName,
                            TestProjectId = requirement.TestProjectId,
                            TeamId = requirement.TeamId,
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

        #endregion Folders

        #region Requirement Specifications

        /// <summary>
        /// Adds a new requirement specification
        /// </summary>
        /// <param name="principal">User making changes</param>
        /// <param name="specification">Entity to save</param>
        /// <returns></returns>
        public async Task AddRequirementSpecificationAsync(ClaimsPrincipal principal, RequirementSpecification specification)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.RequirementSpecification, PermissionLevel.Write);
            specification.TenantId = principal.GetTenantIdOrThrow();

            specification.Created = DateTimeOffset.UtcNow;
            specification.Modified = DateTimeOffset.UtcNow;
            specification.CreatedBy = principal.Identity?.Name;
            specification.ModifiedBy = principal.Identity?.Name;
            await _repository.AddRequirementSpecificationAsync(specification);

            foreach (var observer in _requirementObservers)
            {
                await observer.OnSpecificationCreatedAsync(specification);
            }
        }

        public async Task DeleteRequirementSpecificationAsync(ClaimsPrincipal principal, RequirementSpecification specification)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.RequirementSpecification, PermissionLevel.Delete);
            principal.ThrowIfEntityTenantIsDifferent(specification);

            await _repository.DeleteRequirementSpecificationAsync(specification);


            foreach (var observer in _requirementObservers)
            {
                await observer.OnSpecificationDeletedAsync(specification);
            }
        }

        public async Task DeleteSpecificationRequirementsAndFoldersAsync(ClaimsPrincipal principal, RequirementSpecification specification)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.RequirementSpecification, PermissionLevel.Delete);
            principal.ThrowIfEntityTenantIsDifferent(specification);
            await _repository.DeleteSpecificationRequirementsAndFoldersAsync(specification);
        }


        /// <summary>
        /// Searches for requirement specifications
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<PagedResult<RequirementSpecification>> SearchRequirementSpecificationsAsync(ClaimsPrincipal principal, SearchQuery query)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.RequirementSpecification, PermissionLevel.Read);

            var filters = Specifications.RequirementSpecificationBuilder.From(query);
            filters = [.. filters, new FilterByTenant<RequirementSpecification>(principal.GetTenantIdOrThrow())];

            return await _repository.SearchRequirementSpecificationsAsync(filters, query.Offset, query.Count);
        }
        public async Task<PagedResult<RequirementSpecification>> SearchRequirementSpecificationsAsync(ClaimsPrincipal principal, FilterSpecification<RequirementSpecification>[] filters, int offset, int count)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.RequirementSpecification, PermissionLevel.Read);
            return await _repository.SearchRequirementSpecificationsAsync(filters, offset, count);
        }

        public async Task<RequirementSpecification?> GetRequirementSpecificationBySlugAsync(ClaimsPrincipal principal, string slug)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.RequirementSpecification, PermissionLevel.Read);

            FilterSpecification<RequirementSpecification>[] filters = [
                new FilterByTenant<RequirementSpecification>(principal.GetTenantIdOrThrow()),
                new FilterRequirementSpecificationBySlug(slug)
                ];

            var result = await _repository.SearchRequirementSpecificationsAsync(filters, 0, 1);
            return result.Items.FirstOrDefault();
        }

        public async Task<RequirementSpecification?> GetRequirementSpecificationByIdAsync(ClaimsPrincipal principal, long id)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.RequirementSpecification, PermissionLevel.Read);

            FilterSpecification<RequirementSpecification>[] filters = [
                new FilterByTenant<RequirementSpecification>(principal.GetTenantIdOrThrow()),
                new FilterRequirementSpecificationById(id)
                ];

            var result = await _repository.SearchRequirementSpecificationsAsync(filters, 0, 1);
            return result.Items.FirstOrDefault();
        }

        /// <summary>
        /// Updates a requirement specification
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="specification"></param>
        /// <returns></returns>
        public async Task UpdateRequirementSpecificationAsync(ClaimsPrincipal principal, RequirementSpecification specification)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.RequirementSpecification, PermissionLevel.Write);
            principal.ThrowIfEntityTenantIsDifferent(specification);
            specification.ModifiedBy = principal.Identity?.Name;
            specification.Modified = DateTimeOffset.UtcNow;
            await _repository.UpdateRequirementSpecificationAsync(specification);

            foreach (var observer in _requirementObservers)
            {
                await observer.OnSpecificationSavedAsync(specification);
            }
        }
        #endregion Requirement Specifications

        public async Task<TraceabilityNode> DiscoverTraceabilityAsync(ClaimsPrincipal principal, Requirement requirement, int depth)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.Requirement, PermissionLevel.Read);

            return await _mediator.Send(new DiscoverRequirementRelationshipsRequest(principal, requirement, depth));
        }
    }
}
