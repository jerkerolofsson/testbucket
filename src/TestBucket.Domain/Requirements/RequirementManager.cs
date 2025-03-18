using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Requirements
{
    public class RequirementManager : IRequirementManager
    {
        private readonly IRequirementRepository _repository;

        public RequirementManager(IRequirementRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Adds a new requirement specification
        /// </summary>
        /// <param name="spec"></param>
        /// <returns></returns>
        public Task AddRequirementSpecificationAsync(ClaimsPrincipal principal, RequirementSpecification specification)
        {
            var tenantId = principal.GetTentantIdOrThrow();
            specification.Created = DateTimeOffset.UtcNow;
            specification.Modified = DateTimeOffset.UtcNow;
            specification.CreatedBy = principal.Identity?.Name;
            specification.ModifiedBy = principal.Identity?.Name;
            return _repository.AddRequirementSpecificationAsync(specification);
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
            filters = [.. filters, new FilterByTenant<RequirementSpecification>(principal.GetTentantIdOrThrow())];

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
