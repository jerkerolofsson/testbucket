using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Requirements;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Data.Requirements
{
    internal class RequirementRepository : IRequirementRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public RequirementRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<PagedResult<RequirementSpecification>> SearchRequirementSpecificationsAsync(string tenantId, SearchQuery query)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var suites = dbContext.RequirementSpecifications.Where(x => x.TenantId == tenantId);

            // Apply filter
            if (query.TeamId is not null)
            {
                suites = suites.Where(x => x.TeamId == query.TeamId);
            }
            if (query.ProjectId is not null)
            {
                suites = suites.Where(x => x.TestProjectId == query.ProjectId);
            }
            if (query.Text is not null)
            {
                suites = suites.Where(x => x.Name.ToLower().Contains(query.Text.ToLower()));
            }

            long totalCount = await suites.LongCountAsync();
            var items = suites.OrderBy(x => x.Name).Skip(query.Offset).Take(query.Count);

            return new PagedResult<RequirementSpecification>
            {
                TotalCount = totalCount,
                Items = items.ToArray()
            };
        }
        public async Task AddRequirementSpecificationAsync(string tenantId, RequirementSpecification spec)
        {
            spec.TenantId = tenantId;
            spec.Created = DateTimeOffset.UtcNow;

            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            await dbContext.RequirementSpecifications.AddAsync(spec);
            await dbContext.SaveChangesAsync();
        }
    }
}
