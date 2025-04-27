using System.Linq;
using System.Security.Claims;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

using TestBucket.Contracts.Fields;
using TestBucket.Domain.Fields.Specifications;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Fields
{
    public class FieldDefinitionManager : IFieldDefinitionManager
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IFieldRepository _fieldRepository;

        public FieldDefinitionManager(
            IMemoryCache memoryCache,
            IFieldRepository fieldRepository)
        {
            _memoryCache = memoryCache;
            _fieldRepository = fieldRepository;
        }

        public async Task AddAsync(ClaimsPrincipal principal, FieldDefinition fieldDefinition)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Write);

            var cacheKey = GetCacheKey(principal);
            _memoryCache.Remove(cacheKey);

            fieldDefinition.TenantId = principal.GetTenantIdOrThrow();

            await _fieldRepository.AddAsync(fieldDefinition);
        }

        private static string GetCacheKey(ClaimsPrincipal principal)
        {
            return "fielddefinitions:" + principal.GetTenantIdOrThrow();
        }

        public async Task UpdateAsync(ClaimsPrincipal principal, FieldDefinition fieldDefinition)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Write);

            var cacheKey = GetCacheKey(principal);
            _memoryCache.Remove(cacheKey);

            fieldDefinition.TenantId = principal.GetTenantIdOrThrow();

            await _fieldRepository.UpdateAsync(fieldDefinition);
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

            var cacheKey = GetCacheKey(principal);
            _memoryCache.Remove(cacheKey);

            fieldDefinition.TenantId = principal.GetTenantIdOrThrow();
            await _fieldRepository.DeleteAsync(fieldDefinition);
        }

        public async Task<IReadOnlyList<FieldDefinition>> SearchAsync(ClaimsPrincipal principal, SearchFieldQuery query)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Read);

            var cacheKey = GetCacheKey(principal);
            var fieldDefinitions = (await _memoryCache.GetOrCreateAsync(cacheKey, async (e) =>
            {
                var tenantId = principal.GetTenantIdOrThrow();
                var definitions = await GetAllFilterDefinitionsForTenantAsync(tenantId);
                e.AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(15);

                return definitions;
            }) ?? []).AsQueryable();

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

        public async Task UpsertTestRunFieldAsync(ClaimsPrincipal principal, TestRunField field)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.TestRun, PermissionLevel.Write);

            field.TenantId = principal.GetTenantIdOrThrow();
            await _fieldRepository.UpsertTestRunFieldAsync(field);
        }
        public async Task UpsertTestCaseFieldAsync(ClaimsPrincipal principal, TestCaseField field)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.TestCase, PermissionLevel.Write);

            field.TenantId = principal.GetTenantIdOrThrow();
            await _fieldRepository.UpsertTestCaseFieldAsync(field);
        }
    }
}
