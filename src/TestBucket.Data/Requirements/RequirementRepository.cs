﻿using OpenTelemetry.Trace;

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


        public async Task<RequirementSpecification?> GetRequirementSpecificationByIdAsync(long requirementSpecificationId)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            return await dbContext.RequirementSpecifications.Where(x=>x.Id == requirementSpecificationId).FirstOrDefaultAsync();
        }

        public async Task DeleteRequirementSpecificationAsync(RequirementSpecification specification)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            // Delete requirements and links
            foreach (var requirement in dbContext.Requirements.Where(x => x.RequirementSpecificationId == specification.Id))
            {
                foreach (var link in dbContext.RequirementTestLinks.Where(x => x.RequirementId == requirement.Id))
                {
                    dbContext.RequirementTestLinks.Remove(link);
                }

                dbContext.Requirements.Remove(requirement);
            }

            // Delete folders
            foreach (var folder in dbContext.RequirementSpecificationFolders.Where(x => x.RequirementSpecificationId == specification.Id))
            {
                dbContext.RequirementSpecificationFolders.Remove(folder);
            }

            // Delete the spec!
            dbContext.RequirementSpecifications.Remove(specification);

            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteSpecificationRequirementsAndFoldersAsync(RequirementSpecification specification)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            // Delete requirements and links
            foreach (var requirement in dbContext.Requirements.Where(x => x.RequirementSpecificationId == specification.Id))
            {
                foreach(var link in dbContext.RequirementTestLinks.Where(x=>x.RequirementId == requirement.Id))
                {
                    dbContext.RequirementTestLinks.Remove(link);
                }

                dbContext.Requirements.Remove(requirement);
            }

            // Delete folders
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

            await CalculatePathAsync(dbContext, requirement);

            await dbContext.Requirements.AddAsync(requirement);
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateRequirementAsync(Requirement requirement)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            await CalculatePathAsync(dbContext, requirement);

            dbContext.Requirements.Update(requirement);
            await dbContext.SaveChangesAsync();
        }

        public async Task<List<RequirementTestLink>> GetRequirementLinksForSpecificationAsync(string tenantId, long requirementSpecificationId)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            return await dbContext.RequirementTestLinks.AsNoTracking().Where(link => link.TenantId == tenantId && link.RequirementSpecificationId == requirementSpecificationId).ToListAsync();
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

        public async Task<RequirementSpecificationFolder[]> SearchRequirementFoldersAsync(FilterSpecification<RequirementSpecificationFolder>[] filters)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var items = dbContext.RequirementSpecificationFolders
                .AsQueryable();

            foreach (var filter in filters)
            {
                items = items.Where(filter.Expression);
            }

            return await items.ToArrayAsync();
        }

        public async Task AddRequirementFolderAsync(RequirementSpecificationFolder folder)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            await dbContext.RequirementSpecificationFolders.AddAsync(folder);
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateRequirementFolderAsync(RequirementSpecificationFolder folder)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            var existingFolder = await dbContext.RequirementSpecificationFolders.AsNoTracking().Where(x => x.Id == folder.Id).FirstOrDefaultAsync();
            if (existingFolder is null)
            {
                throw new InvalidOperationException("Folder does not exist!");
            }


            var hasPathChanged = existingFolder.Name != folder.Name || existingFolder.ParentId != folder.ParentId || existingFolder.RequirementSpecificationId != folder.RequirementSpecificationId;
            if (hasPathChanged)
            {
                await CalculatePathAsync(dbContext, folder);
            }

            dbContext.RequirementSpecificationFolders.Update(folder);
            await dbContext.SaveChangesAsync();

            if (hasPathChanged)
            {
                await UpdateChildRequirementFolderPathsAsync(dbContext, folder.Id, folder.RequirementSpecificationId);
                await dbContext.SaveChangesAsync();
            }
        }

        #region Path

        private async Task UpdateChildRequirementFolderPathsAsync(ApplicationDbContext dbContext, long id, long requirementSpecificationId)
        {
            foreach (var folder in await dbContext.RequirementSpecificationFolders.Where(x => x.ParentId == id).ToListAsync())
            {
                await CalculatePathAsync(dbContext, folder);
                await UpdateChildRequirementFolderPathsAsync(dbContext, folder.Id, requirementSpecificationId);
            }

            foreach (var requirement in await dbContext.Requirements.Where(x => x.RequirementSpecificationFolderId == id).ToListAsync())
            {
                // If the folder has been moved to another specification, update the spec id
                requirement.RequirementSpecificationId = requirementSpecificationId;
                await CalculatePathAsync(dbContext, requirement);
            }
        }

        private async Task CalculatePathAsync(ApplicationDbContext dbContext, long? folderId, List<string> pathComponents, List<long> pathIds)
        {
            while (folderId is not null)
            {
                var folder = await dbContext.RequirementSpecificationFolders.AsNoTracking().Where(x => x.Id == folderId).FirstOrDefaultAsync();
                if (folder is null)
                {
                    return;
                }
                pathComponents.Add(folder.Name);
                pathIds.Add(folder.Id);
                folderId = folder.ParentId;
            }
        }

        private async Task CalculatePathAsync(ApplicationDbContext dbContext, Requirement requirement)
        {
            List<string> pathComponents = new();
            List<long> pathIds = new();
            await CalculatePathAsync(dbContext, requirement.RequirementSpecificationFolderId, pathComponents, pathIds);
            pathComponents.Reverse();
            pathIds.Reverse();
            requirement.Path = string.Join('/', pathComponents);
            requirement.PathIds = pathIds.ToArray();
        }

        private async Task CalculatePathAsync(ApplicationDbContext dbContext, RequirementSpecificationFolder folder)
        {
            List<string> pathComponents = new();
            List<long> pathIds = new();
            await CalculatePathAsync(dbContext, folder.ParentId, pathComponents, pathIds);
            pathComponents.Reverse();
            pathIds.Reverse();
            folder.Path = string.Join('/', pathComponents);
            folder.PathIds = pathIds.ToArray();
        }


        #endregion Path
    }
}
