using System.Security.Claims;
using TestBucket.Domain.Fields.Models;
using TestBucket.Domain.Fields.Specifications;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Fields
{
    public class FieldDefinitionManager : IFieldDefinitionManager
    {
        private readonly IFieldRepository _fieldRepository;

        public FieldDefinitionManager(IFieldRepository fieldRepository)
        {
            _fieldRepository = fieldRepository;
        }

        public async Task AddAsync(ClaimsPrincipal principal, FieldDefinition fieldDefinition)
        {
            fieldDefinition.TenantId = principal.GetTentantIdOrThrow();

            await _fieldRepository.AddAsync(fieldDefinition);
        }

        public async Task UpdateAsync(ClaimsPrincipal principal, FieldDefinition fieldDefinition)
        {
            fieldDefinition.TenantId = principal.GetTentantIdOrThrow();

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
            var specifications = FieldSpecificationBuilder.From(new SearchFieldQuery
            {
                Target = target,
                ProjectId = testProjectId
            });
            return await SearchAsync(principal, specifications);
        }
        public async Task DeleteAsync(ClaimsPrincipal principal, FieldDefinition fieldDefinition)
        {
            fieldDefinition.TenantId = principal.GetTentantIdOrThrow();

            await _fieldRepository.DeleteAsync(fieldDefinition);
        }
        public async Task<IReadOnlyList<FieldDefinition>> SearchAsync(ClaimsPrincipal principal, IReadOnlyList<FilterSpecification<FieldDefinition>> specifications)
        {
            var tenantId = principal.GetTentantIdOrThrow();
            specifications = [new FilterByTenant<FieldDefinition>(tenantId), .. specifications];

            return await _fieldRepository.SearchAsync(specifications);
        }


        public async Task UpsertTestCaseFieldsAsync(ClaimsPrincipal principal, TestCaseField field)
        {
            field.TenantId = principal.GetTentantIdOrThrow();
            await _fieldRepository.UpsertTestCaseFieldsAsync(field);
        }
    }
}
