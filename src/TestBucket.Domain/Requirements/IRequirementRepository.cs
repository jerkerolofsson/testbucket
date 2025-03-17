using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Requirements.Models;

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
        Task AddRequirementSpecificationAsync(string tenantId, RequirementSpecification spec);

        /// <summary>
        /// Searches for requirement specifications
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<PagedResult<RequirementSpecification>> SearchRequirementSpecificationsAsync(string tenantId, SearchQuery query);
    }
}
