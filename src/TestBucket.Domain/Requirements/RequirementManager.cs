using Mediator;

using Microsoft.Extensions.Logging;

using TestBucket.Contracts.Fields;
using TestBucket.Contracts.Requirements;
using TestBucket.Contracts.Requirements.Types;
using TestBucket.Domain.AI.Embeddings;
using TestBucket.Domain.Features.Traceability.Models;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Fields.Handlers;
using TestBucket.Domain.Progress;
using TestBucket.Domain.Requirements.Events;
using TestBucket.Domain.Requirements.Import;
using TestBucket.Domain.Requirements.Mapping;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Requirements.Specifications;
using TestBucket.Domain.Requirements.Specifications.Folders;
using TestBucket.Domain.Requirements.Specifications.Links;
using TestBucket.Domain.Requirements.Specifications.Requirements;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Features.Traceability;
using TestBucket.Traits.Core;

namespace TestBucket.Domain.Requirements
{
    public class RequirementManager : IRequirementManager
    {
        private readonly IRequirementRepository _repository;
        private readonly IRequirementImporter _importer;
        private readonly IFieldManager _fieldManager;
        private readonly List<IRequirementObserver> _requirementObservers = new();
        private readonly IMediator _mediator;
        private readonly TimeProvider _timeProvider;
        private readonly ILogger<RequirementManager> _logger;

        public RequirementManager(IRequirementRepository repository, IMediator mediator, TimeProvider timeProvider, IRequirementImporter importer, IFieldManager fieldManager, ILogger<RequirementManager> logger)
        {
            _repository = repository;
            _mediator = mediator;
            _timeProvider = timeProvider;
            _importer = importer;
            _fieldManager = fieldManager;
            _logger = logger;
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

        public async Task SetRequirementTypeAsync(ClaimsPrincipal principal, long[] requirementIds, RequirementType requirementType, ProgressTask progress)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.Requirement, PermissionLevel.Read);
            var tenantId = principal.GetTenantIdOrThrow();
            var userName = principal.Identity?.Name ?? throw new UnauthorizedAccessException("User identity missing");

            foreach (var item in requirementIds.Index())
            {
                var requirement = await _repository.GetRequirementByIdAsync(tenantId, item.Item);
                if(requirement is not null)
                {
                    principal.ThrowIfEntityTenantIsDifferent(requirement);
                    await progress.ReportStatusAsync($"Updating {requirement.Name}", item.Index * 100.0 / (double)requirementIds.Length);

                    if(requirement.MappedType == requirementType.MappedType && requirement.RequirementType == requirementType.Name)
                    {
                        continue;
                    }

                    requirement.RequirementType = requirementType.Name;
                    requirement.MappedState = requirement.MappedState;
                    requirement.Modified = _timeProvider.GetUtcNow();
                    requirement.ModifiedBy = userName;
                    await _repository.UpdateRequirementAsync(requirement);
                }
            }
        }


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
        /// Called when a requirement field has changed
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public async Task OnRequirementFieldChangedAsync(ClaimsPrincipal principal, RequirementField field)
        {
            var requirement = await GetRequirementByIdAsync(principal,field.RequirementId);
            if(requirement is null)
            {
                return;
            }

            // Trigger update of timestamp
            requirement.Modified = _timeProvider.GetUtcNow();

            await GenerateEmbeddingAsync(principal, requirement, requirement.RequirementFields);

            await _repository.UpdateRequirementAsync(requirement);

            foreach (var observer in _requirementObservers)
            {
                await observer.OnRequirementFieldChangedAsync(requirement);
            }
        }

        private async Task GenerateEmbeddingAsync(ClaimsPrincipal principal, Requirement item, IEnumerable<RequirementField>? fields)
        {
            if (item.TestProjectId is null)
            {
                return;
            }

            try
            {
                var text = $"{item.Name} {item.Description}";

                if(fields is not null)
                {
                    foreach(var field in fields)
                    {
                        if (field.FieldDefinition is not null)
                        {
                            text += $"\n{field.FieldDefinition.Name}={field.GetValueAsString()}";
                        }
                    }
                }

                var response = await _mediator.Send(new GenerateEmbeddingRequest(principal, item.TestProjectId.Value, text));
                if (response.EmbeddingVector is not null)
                {
                    item.Embedding = new Pgvector.Vector(response.EmbeddingVector.Value);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating embedding for component {RequirementName}", item.Name);
            }
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

            if (requirement.RequirementType is not null)
            {
                requirement.MappedType = RequirementTypeConverter.GetMappedRequirementTypeFromString(requirement.RequirementType);
            }

            // Update fields
            requirement.ModifiedBy = principal.Identity?.Name;
            requirement.Modified = _timeProvider.GetUtcNow();

            var existing = await _repository.GetRequirementByIdAsync(tenantId, requirement.Id);
            var isDescriptionChanged = existing?.Description != requirement.Description || existing?.Name != requirement.Name;
            if (isDescriptionChanged || requirement.Embedding is null)
            {
                await GenerateEmbeddingAsync(principal, requirement, requirement.RequirementFields ?? existing?.RequirementFields);
            }

            await _repository.UpdateRequirementAsync(requirement);

            // Notify observers
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
        /// Semantic search for requirements
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<PagedResult<Requirement>> SemanticSearchRequirementsAsync(ClaimsPrincipal principal, SearchRequirementQuery query)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.Requirement, PermissionLevel.Read);

            // Get the text and temporarily remove it from the query to
            // prevent the text filter to be created
            var semanticSearchText = query.Text;
            query.Text = null;
            try
            {

                var filters = RequirementSpecificationBuilder.From(query);
                filters = [.. filters, new FilterByTenant<Requirement>(principal.GetTenantIdOrThrow())];

                if (!string.IsNullOrEmpty(semanticSearchText) && query.ProjectId is not null)
                {
                    var embedding = await _mediator.Send(new GenerateEmbeddingRequest(principal, query.ProjectId.Value, semanticSearchText));
                    if (embedding?.EmbeddingVector is not null)
                    {
                        return await _repository.SemanticSearchRequirementsAsync(embedding.EmbeddingVector.Value, filters, query.Offset, query.Count);
                    }
                }
            }
            finally
            {
                // Restore the text filter
                query.Text = semanticSearchText;
            }

            return await SearchRequirementsAsync(principal, query);
        }


        public async Task<long[]> SearchRequirementIdsAsync(ClaimsPrincipal principal, FilterSpecification<Requirement>[] filters)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.Requirement, PermissionLevel.Read);

            filters = [.. filters, new FilterByTenant<Requirement>(principal.GetTenantIdOrThrow())];

            return await _repository.SearchRequirementIdsAsync(filters);
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
        /// Gets a requirement by externalid
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Requirement?> GetRequirementByExternalIdAsync(ClaimsPrincipal principal, string id)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.Requirement, PermissionLevel.Read);

            FilterSpecification<Requirement>[] filters = [new FilterByTenant<Requirement>(principal.GetTenantIdOrThrow()), new FilterRequirementByExternalId(null, id)];

            var result = await _repository.SearchRequirementsAsync(filters, 0, 1);
            return result.Items.FirstOrDefault();
        }

        /// <summary>
        /// Gets a requirement by slug
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="slug"></param>
        /// <returns></returns>
        public async Task<Requirement?> GetRequirementBySlugAsync(ClaimsPrincipal principal, long projectId, string slug)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.Requirement, PermissionLevel.Read);

            FilterSpecification<Requirement>[] filters = [
                new FilterByTenant<Requirement>(principal.GetTenantIdOrThrow()),
                new FilterByProject<Requirement>(projectId),
                new FilterRequirementBySlug(slug)];

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
            requirement.Created = _timeProvider.GetUtcNow();
            requirement.Modified = _timeProvider.GetUtcNow();
            requirement.CreatedBy = principal.Identity?.Name;
            requirement.ModifiedBy = principal.Identity?.Name;

            await GenerateFoldersFromPathAsync(requirement);
            await GenerateEmbeddingAsync(principal, requirement, requirement.RequirementFields);

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
            ArgumentNullException.ThrowIfNull(requirement);
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

        public async Task DeleteSearchFolderAsync(ClaimsPrincipal principal, RequirementSpecification collection, SearchFolder searchFolder)
        {
            if(collection.SearchFolders is not null && collection.SearchFolders.Remove(searchFolder))
            {
                await UpdateRequirementSpecificationAsync(principal, collection);
            }
        }

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

            specification.Created = _timeProvider.GetUtcNow();
            specification.Modified = _timeProvider.GetUtcNow();
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

            var filters = RequirementSpecificationBuilder.From(query);
            filters = [.. filters, new FilterByTenant<RequirementSpecification>(principal.GetTenantIdOrThrow())];

            if(!string.IsNullOrEmpty(query.Text))
            {
                filters = [.. filters, new FilterRequirementSpecificationByText(query.Text)];
            }

            return await _repository.SearchRequirementSpecificationsAsync(filters, query.Offset, query.Count);
        }
        public async Task<PagedResult<RequirementSpecification>> SearchRequirementSpecificationsAsync(ClaimsPrincipal principal, FilterSpecification<RequirementSpecification>[] filters, int offset, int count)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.RequirementSpecification, PermissionLevel.Read);
            return await _repository.SearchRequirementSpecificationsAsync(filters, offset, count);
        }

        public async Task<RequirementSpecification?> GetRequirementSpecificationBySlugAsync(ClaimsPrincipal principal, long projectId, string slug)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.RequirementSpecification, PermissionLevel.Read);

            FilterSpecification<RequirementSpecification>[] filters = [
                new FilterByTenant<RequirementSpecification>(principal.GetTenantIdOrThrow()),
                new FilterByProject<RequirementSpecification>(projectId),
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
            specification.Modified = _timeProvider.GetUtcNow();
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
        public async Task ApproveRequirementAsync(ClaimsPrincipal principal, Requirement requirement)
        {
            await _mediator.Send(new ApproveRequirementRequest(principal, requirement, true));
        }

        private async Task RestoreRequirementLinks(ClaimsPrincipal principal, List<RequirementTestLink> oldLinks, Requirement requirement)
        {
            if (requirement.ExternalId is not null)
            {
                var requirementLinks = oldLinks.Where(x => x.RequirementExternalId == requirement.ExternalId).ToList();
                foreach (var link in requirementLinks)
                {
                    link.RequirementId = requirement.Id;
                    await AddRequirementLinkAsync(principal, link);
                }
            }
        }
        public async Task ExtractRequirementsFromSpecificationAsync(ClaimsPrincipal principal, RequirementSpecification specification, CancellationToken cancellationToken)
        {

            var requirements = await _importer.ExtractRequirementsAsync(specification, cancellationToken);

            // Get a copy of all existing links for the specification, as we are
            // deleting requirements, we will re-create these based on the requirement ExternalID property later
            var oldLinks = await GetRequirementLinksForSpecificationAsync(principal, specification);

            // Delete all old requirements, and folders
            await DeleteSpecificationRequirementsAndFoldersAsync(principal, specification);

            // Import all new
            foreach (var requirement in requirements)
            {
                cancellationToken.ThrowIfCancellationRequested();

                requirement.TestProjectId = specification.TestProjectId;
                requirement.TeamId = specification.TeamId;
                requirement.RequirementSpecificationId = specification.Id;
                await AddRequirementAsync(principal, requirement);

                await RestoreRequirementLinks(principal, oldLinks, requirement);
            }
        }

        public async Task ImportAsync(ClaimsPrincipal principal, TestProject project, List<RequirementEntityDto> entities)
        {
            var importedEntities = new List<RequirementEntity>();

            // First import all specifications
            Dictionary<string, long> specificationMap = [];

            foreach (var entity in entities)
            {
                if (entity is RequirementSpecificationDto specificationDto)
                {
                    var specification = specificationDto.ToDbo();


                    specification.TestProjectId = project.Id;
                    specification.TeamId = project.TeamId;
                    specification.TenantId = project.TenantId;

                    // Get existing specification if possible
                    bool add = false;
                    if (specification.Slug is not null)
                    {
                        var existingSpecification = await GetRequirementSpecificationBySlugAsync(principal, project.Id, specification.Slug);
                        if (existingSpecification is not null)
                        {
                            specificationMap[specification.Slug] = existingSpecification.Id;
                            add = false;
                        }
                        else
                        {
                            add = true;
                        }
                    }
                    else
                    {
                        add = true;
                    }

                    if (add)
                    {
                        await AddRequirementSpecificationAsync(principal, specification);
                        if (specification.Slug is null)
                        {
                            throw new Exception("Expected slug to be set after specification was added");
                        }
                        importedEntities.Add(specification);
                        specificationMap[specification.Slug] = specification.Id;
                    }
                }
            }

            foreach (var entity in entities)
            {
                if (entity is RequirementDto requirementDto && specificationMap.Count > 0 && requirementDto.Slug is not null)
                {
                    long specificationId = specificationMap.First().Value;
                    if (requirementDto.SpecificationSlug is not null &&
                        specificationMap.TryGetValue(requirementDto.SpecificationSlug, out var specificationIdFromSlug))
                    {
                        specificationId = specificationIdFromSlug;
                    }
                    var requirement = requirementDto.ToDbo();
                    requirement.RequirementSpecificationId = specificationId;
                    requirement.TestProjectId = project.Id;
                    requirement.TeamId = project.TeamId;
                    requirement.TenantId = project.TenantId;

                    Requirement? existingRequirement = await GetRequirementBySlugAsync(principal, project.Id, requirementDto.Slug);
                    if (existingRequirement is null)
                    {
                        await AddRequirementAsync(principal, requirement);

                        // Fields
                        if (requirementDto.Traits?.Traits is not null && requirementDto.Traits.Traits.Count > 0)
                        {
                            foreach (var trait in requirementDto.Traits.Traits)
                            {
                                var fieldDefinition = await _mediator.Send(new ImportTraitRequest(principal, project.Id, trait, FieldTarget.Requirement));
                                var field = new RequirementField { FieldDefinitionId = fieldDefinition.Id, RequirementId = requirement.Id };
                                FieldValueConverter.TryAssignValue(fieldDefinition, field, [trait.Value]);
                                await _fieldManager.UpsertRequirementFieldAsync(principal, field);
                            }
                        }

                        importedEntities.Add(requirement);
                    }
                }
            }

            // Links between requirements
            foreach (var entity in entities)
            {
                if (entity is RequirementDto requirementDto && requirementDto.Slug is not null)
                {
                    if (!string.IsNullOrEmpty(requirementDto.ParentRequirementSlug))
                    {
                        Requirement? existingRequirement = await GetRequirementBySlugAsync(principal, requirementDto.Slug);
                        Requirement? parentRequirement = await GetRequirementBySlugAsync(principal, requirementDto.ParentRequirementSlug);
                        if (existingRequirement is not null && parentRequirement is not null)
                        {
                            existingRequirement.ParentRequirementId = parentRequirement.Id;
                            await UpdateRequirementAsync(principal, existingRequirement);
                        }
                    }
                }
            }
        }
    }
}
