using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Requirements;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Shared.Specifications;
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

        public async Task<PagedResult<Requirement>> SearchRequirementsAsync(IEnumerable<FilterSpecification<Requirement>> filters, int offset, int count)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var requirements = dbContext.Requirements.AsQueryable();

            foreach (var filter in filters)
            {
                requirements = requirements.Where(filter.Expression);
            }

            long totalCount = await requirements.LongCountAsync();
            var items = requirements.OrderBy(x => x.Name).Skip(offset).Take(count);

            return new PagedResult<Requirement>
            {
                TotalCount = totalCount,
                Items = items.ToArray()
            };
        }
        public async Task<PagedResult<RequirementSpecification>> SearchRequirementSpecificationsAsync(IEnumerable<FilterSpecification<RequirementSpecification>> filters, int offset, int count)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var requirementSpecifications = dbContext.RequirementSpecifications.AsQueryable();

            foreach(var filter in filters)
            {
                requirementSpecifications = requirementSpecifications.Where(filter.Expression);
            }

            long totalCount = await requirementSpecifications.LongCountAsync();
            var items = requirementSpecifications.OrderBy(x => x.Name).Skip(offset).Take(count);

            return new PagedResult<RequirementSpecification>
            {
                TotalCount = totalCount,
                Items = items.ToArray()
            };
        }
        public async Task AddRequirementSpecificationAsync(RequirementSpecification specification)
        {
            specification.Created = DateTimeOffset.UtcNow;

            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            await dbContext.RequirementSpecifications.AddAsync(specification);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteRequirementSpecificationAsync(RequirementSpecification specification)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            foreach (var requirement in dbContext.Requirements.Where(x => x.RequirementSpecificationId == specification.Id))
            {
                foreach (var link in dbContext.RequirementTestLinks.Where(x => x.RequirementId == requirement.Id))
                {
                    dbContext.RequirementTestLinks.Remove(link);
                }

                dbContext.Requirements.Remove(requirement);
            }
            foreach (var folder in dbContext.RequirementSpecificationFolders.Where(x => x.RequirementSpecificationId == specification.Id))
            {
                dbContext.RequirementSpecificationFolders.Remove(folder);
            }
            dbContext.RequirementSpecifications.Remove(specification);

            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteSpecificationRequirementsAndFoldersAsync(RequirementSpecification specification)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            foreach (var requirement in dbContext.Requirements.Where(x => x.RequirementSpecificationId == specification.Id))
            {
                dbContext.Requirements.Remove(requirement);
            }
            foreach (var folder in dbContext.RequirementSpecificationFolders.Where(x => x.RequirementSpecificationId == specification.Id))
            {
                dbContext.RequirementSpecificationFolders.Remove(folder);
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateRequirementSpecificationAsync(RequirementSpecification specification)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            dbContext.RequirementSpecifications.Update(specification);
            await dbContext.SaveChangesAsync();
        }

        public async Task AddRequirementAsync(Requirement requirement)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            await dbContext.Requirements.AddAsync(requirement);
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateRequirementAsync(Requirement requirement)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            dbContext.Requirements.Update(requirement);
            await dbContext.SaveChangesAsync();
        }

        public async Task AddRequirementLinkAsync(RequirementTestLink link)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            await dbContext.RequirementTestLinks.AddAsync(link);
            await dbContext.SaveChangesAsync();
        }
        public async Task DeleteRequirementLinkAsync(RequirementTestLink link)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            dbContext.RequirementTestLinks.Remove(link);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteRequirementAsync(Requirement requirement)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            foreach (var link in dbContext.RequirementTestLinks.Where(x => x.RequirementId == requirement.Id))
            {
                dbContext.RequirementTestLinks.Remove(link);
            }

            dbContext.Requirements.Remove(requirement);
            await dbContext.SaveChangesAsync();
        }

        public async Task<RequirementTestLink[]> SearchRequirementLinksAsync(FilterSpecification<RequirementTestLink>[] filters)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var requirements = dbContext.RequirementTestLinks
                .Include(x => x.Requirement)
                .Include(x => x.TestCase)
                .AsQueryable();

            foreach (var filter in filters)
            {
                requirements = requirements.Where(filter.Expression);
            }

            return await requirements.ToArrayAsync();
        }
    }
}
