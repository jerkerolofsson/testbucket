using System.Linq;
using System.Security.Claims;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

using TestBucket.Contracts.Fields;
using TestBucket.Contracts.Integrations;
using TestBucket.Domain.Fields.Specifications;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Tenants.Models;
using TestBucket.Domain.Testing.Models;
using TestBucket.Traits.Core;

namespace TestBucket.Domain.Fields
{
    public class FieldDefinitionManager : IFieldDefinitionManager
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IFieldRepository _fieldRepository;
        private readonly IProjectManager _projectManager;
        private readonly IReadOnlyList<IFieldCompletionsProvider> _completionProviders;

        public FieldDefinitionManager(
            IMemoryCache memoryCache,
            IEnumerable<IFieldCompletionsProvider> completionProviders,
            IFieldRepository fieldRepository,
            IProjectManager projectManager)
        {
            _completionProviders = completionProviders.ToList();
            _memoryCache = memoryCache;
            _fieldRepository = fieldRepository;
            _projectManager = projectManager;
        }

        public async Task AddAsync(ClaimsPrincipal principal, FieldDefinition fieldDefinition)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Write);

            fieldDefinition.TenantId = principal.GetTenantIdOrThrow();

            await _fieldRepository.AddAsync(fieldDefinition);

            ClearTenantCache(principal.GetTenantIdOrThrow());
        }

        public async Task UpdateAsync(ClaimsPrincipal principal, FieldDefinition fieldDefinition)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Write);
            fieldDefinition.TenantId = principal.GetTenantIdOrThrow();

            await _fieldRepository.UpdateAsync(fieldDefinition);

            ClearTenantCache(principal.GetTenantIdOrThrow());
        }

        /// <summary>
        /// Returns all field definitions for the specified project and target
        /// </summary>
        /// <param name="testProjectId"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public async Task<IReadOnlyList<FieldDefinition>> GetDefinitionsAsync(ClaimsPrincipal principal, long? testProjectId, FieldTarget target)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Read);
            var query = new SearchFieldQuery
            {
                Target = target,
                ProjectId = testProjectId,
                Offset = 0,
                Count = 200
            };
            return await SearchAsync(principal, query);
        }


        /// <summary>
        /// Returns all field definitions for the specified project
        /// </summary>
        /// <param name="testProjectId"></param>
        /// <returns></returns>
        public async Task<IReadOnlyList<FieldDefinition>> GetDefinitionsAsync(ClaimsPrincipal principal, long? testProjectId)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Read);
            var query = new SearchFieldQuery
            {
                ProjectId = testProjectId,
                Offset = 0,
                Count = 200
            };
            return await SearchAsync(principal, query);
        }
        public async Task DeleteAsync(ClaimsPrincipal principal, FieldDefinition fieldDefinition)
        {
            // Note: This should be Write and not Delete. It is considered a write operation on the project
            principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Write);

            fieldDefinition.TenantId = principal.GetTenantIdOrThrow();
            await _fieldRepository.DeleteAsync(fieldDefinition);

            ClearTenantCache(principal.GetTenantIdOrThrow());
        }

        public async Task<IReadOnlyList<FieldDefinition>> SearchAsync(ClaimsPrincipal principal, SearchFieldQuery query)
        {
            var tenantId = principal.GetTenantIdOrThrow();
            principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Read);

            var cacheKey = GetCacheKey(tenantId);
            var fieldDefinitions = (await _memoryCache.GetOrCreateAsync(cacheKey, async (e) =>
            {
                var definitions = await GetAllFilterDefinitionsForTenantAsync(tenantId);
                e.AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(15);

                return definitions;
            }) ?? []).AsQueryable();

            //if(fieldDefinitions.Count() == 0)
            //{
            //    ClearTenantCache(tenantId);
            //    fieldDefinitions = (await GetAllFilterDefinitionsForTenantAsync(tenantId)).AsQueryable();

            //    ClearTenantCache(tenantId);
            //    fieldDefinitions = (await GetAllFilterDefinitionsForTenantAsync(tenantId)).AsQueryable();
            //}

            // We cached all field definitions, so filter them now
            var filters = FieldSpecificationBuilder.From(query);
            foreach(var filter in filters)
            {
                fieldDefinitions = fieldDefinitions.Where(filter.Expression);
            }
            return fieldDefinitions.ToList();
        }

        private async Task<IReadOnlyList<FieldDefinition>> GetAllFilterDefinitionsForTenantAsync(string tenantId)
        {
            IReadOnlyList<FilterSpecification<FieldDefinition>> specifications = [new FilterByTenant<FieldDefinition>(tenantId)];
            var definitions = await _fieldRepository.SearchAsync(specifications);
            return definitions;
        }

        public async Task<IReadOnlyList<string>> SearchOptionsAsync(ClaimsPrincipal principal, FieldDefinition field, string text, int count, CancellationToken cancellationToken)
        {
            var tenantId = principal.GetTenantIdOrThrow();
            if (field.TestProjectId is null)
            {
                return [];
            }

            List<string> result = [];
            int remaining = count;
            foreach (var provider in _completionProviders)
            {
                var options = await provider.SearchOptionsAsync(principal, field.DataSourceType, field.TestProjectId.Value, text ,remaining, cancellationToken);
                remaining -= options.Count;
                result.AddRange(options);
                if(remaining <= 0)
                {
                    break;
                }
            }
            return result;
        }

        public async Task<IReadOnlyList<string>> GetOptionsAsync(ClaimsPrincipal principal, FieldDefinition field)
        {
            var tenantId = principal.GetTenantIdOrThrow();

            if(field.DataSourceType == FieldDataSourceType.List)
            {
                if (field.Options is not null)
                {
                    return field.Options.ToList();
                }
            }
            else 
            {
                //return await LoadFieldOptionsAsync(principal, field);

                string cacheKey = $"field-options:{tenantId}:{field.Id}:{field.DataSourceType}";
                var result = await _memoryCache.GetOrCreateAsync(cacheKey, async (e) =>
                {
                    e.AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(1);
                    return await LoadFieldOptionsAsync(principal, field);
                });

                return result ?? [];
            }


            return [];
        }

        private async Task<IReadOnlyList<string>> LoadFieldOptionsAsync(
            ClaimsPrincipal principal,
            FieldDefinition field,
            CancellationToken cancellationToken = default)
        {
            if(field.TestProjectId is null)
            {
                return [];
            }

            List<string> result = [];
            foreach(var provider in _completionProviders)
            {
                var options = await provider.GetOptionsAsync(principal, field.DataSourceType, field.TestProjectId.Value, cancellationToken);
                result.AddRange(options);
            }
            return result;
        }

        public void ClearTenantCache(string tenantId)
        {
            var cacheKey = GetCacheKey(tenantId);
            _memoryCache.Remove(cacheKey);
        }
        private static string GetCacheKey(ClaimsPrincipal principal)
        {
            return GetCacheKey(principal.GetTenantIdOrThrow());
        }

        private static string GetCacheKey(string tenantId)
        {
            return "fielddefinitions:" + tenantId;
        }

    }
}
