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
        /// Adds a new requirement specification
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="spec"></param>
        /// <returns></returns>
        Task AddRequirementSpecificationAsync(RequirementSpecification spec);

        /// <summary>
        /// Searches for requirement specifications
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        Task<PagedResult<RequirementSpecification>> SearchRequirementSpecificationsAsync(IEnumerable<FilterSpecification<RequirementSpecification>> filters, int offset, int count);

        /// <summary>
        /// Updates a requirement specification
        /// </summary>
        /// <param name="specification"></param>
        /// <returns></returns>
        Task UpdateRequirementSpecificationAsync(RequirementSpecification specification);
    }
}
