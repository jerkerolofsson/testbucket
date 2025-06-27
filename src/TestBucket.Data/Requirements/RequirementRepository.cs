using Microsoft.EntityFrameworkCore;

using TestBucket.Contracts.Requirements.Types;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Requirements;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Data.Requirements
{
    internal class RequirementRepository : IRequirementRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private readonly SequenceGenerator _sequenceGenerator = new SequenceGenerator();

        public RequirementRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        #region Folders

        public async Task<RequirementSpecificationFolder?> GetRequirementFolderByIdAsync(string tenantId, long folderId)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            return await dbContext.RequirementSpecificationFolders
                .Where(x => x.TenantId == tenantId && x.Id == folderId).FirstOrDefaultAsync();
        }

        public async Task DeleteRequirementFolderAsync(RequirementSpecificationFolder folder)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            RemoveFolder(folder, dbContext);

            await dbContext.SaveChangesAsync();
        }

        private static void RemoveFolder(RequirementSpecificationFolder folder, ApplicationDbContext dbContext)
        {
            foreach (var requirement in dbContext.Requirements.Where(x => x.RequirementSpecificationFolderId == folder.Id))
            {
                foreach (var link in dbContext.RequirementTestLinks.Where(x => x.RequirementId == requirement.Id))
                {
                    dbContext.RequirementTestLinks.Remove(link);
                }
                foreach (var comment in dbContext.Comments.Where(x => x.RequirementId == requirement.Id))
                {
                    dbContext.Comments.Remove(comment);
                }
                dbContext.Requirements.Remove(requirement);
            }

            // Delete child folders
            foreach (var childFolder in dbContext.RequirementSpecificationFolders.Where(x => x.ParentId == folder.Id))
            {
                RemoveFolder(childFolder, dbContext);
            }

            dbContext.RequirementSpecificationFolders.Remove(folder);
        }
        #endregion

        public async Task<PagedResult<Requirement>> SearchRequirementsAsync(IEnumerable<FilterSpecification<Requirement>> filters, int offset, int count)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var requirements = dbContext.Requirements
                .Include(x=>x.RequirementFields!).ThenInclude(x=>x.FieldDefinition)
                .Include(x => x.Comments)
                .Include(x=>x.TestLinks).AsQueryable();

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
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            if (string.IsNullOrEmpty(specification.Slug) && specification.TenantId is not null)
            {
                specification.Slug = await GenerateSpecificationSlugAsync(specification.TenantId, specification.Name);
            }
            await dbContext.RequirementSpecifications.AddAsync(specification);
            await dbContext.SaveChangesAsync();
        }

        private async Task<string> GetTypeNameAsync(ApplicationDbContext dbContext, Requirement requirement)
        {
            if (requirement.RequirementType == RequirementTypes.Task || requirement.MappedType == MappedRequirementType.Task)
            {
                return "TSK";
            }
            if (requirement.RequirementType == RequirementTypes.Epic || requirement.MappedType == MappedRequirementType.Epic)
            {
                return "EPIC";
            }
            if (requirement.RequirementType == RequirementTypes.Initiative || requirement.MappedType == MappedRequirementType.Initiative)
            {
                return "IN";
            }
            if (requirement.RequirementType == RequirementTypes.Story || requirement.MappedType == MappedRequirementType.Story)
            {
                return "STRY";
            }
            if (requirement.RequirementType == RequirementTypes.DesignDocument || requirement.MappedType == MappedRequirementType.DesignDocument)
            {
                return "DSGN";
            }
            if (requirement.RequirementType == RequirementTypes.Standard || requirement.MappedType == MappedRequirementType.Standard)
            {
                return "STD";
            }
            if (requirement.RequirementType == RequirementTypes.Documentation || requirement.MappedType == MappedRequirementType.Documentation)
            {
                return "DOC";
            }
            if (requirement.RequirementType == RequirementTypes.UserManual || requirement.MappedType == MappedRequirementType.UserManual)
            {
                return "MAN";
            }

            var spec = await dbContext.RequirementSpecifications.AsNoTracking().Where(x => x.Id == requirement.RequirementSpecificationId).FirstOrDefaultAsync();
            if(spec?.SpecificationType is not null)
            {
                var type = spec.SpecificationType;
                foreach(var name in new string[] { "PRS", "SRS", "FRS", "BRS", "DOC", "TASK" })
                {
                    if (type.Contains(name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return name;
                    }
                }
            }

            return "REQ";
        }
      

        private async ValueTask AssignExternalIdAsync(ApplicationDbContext dbContext, Requirement requirement)
        {
            // Sequence number only used for internal issues
            if (requirement.TestProjectId is not null && requirement.TenantId is not null)
            {
                var project = await dbContext.Projects.AsNoTracking().Where(x => x.Id == requirement.TestProjectId).FirstAsync();
                var typeName = await GetTypeNameAsync(dbContext, requirement);

                requirement.SequenceNumber = await _sequenceGenerator.GenerateSequenceNumberAsync(requirement.TenantId, requirement.TestProjectId.Value, "requirement", GetMaxSequenceNumber, default);
                requirement.ExternalId = project.ShortName + "-" + typeName + "-" + requirement.SequenceNumber;
            }
        }

        private async ValueTask<int> GetMaxSequenceNumber(string tenantId, long projectId, CancellationToken cancellationToken)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            var lastestSequenceNumber = await dbContext.Requirements.AsNoTracking()
                .Where(x => x.TenantId == tenantId && x.TestProjectId == projectId && x.SequenceNumber != null)
                .OrderByDescending(x => x.SequenceNumber).Select(x => x.SequenceNumber).Take(1).FirstOrDefaultAsync();

            return lastestSequenceNumber ?? 0;
        }

        public async Task<RequirementSpecification?> GetRequirementSpecificationByIdAsync(string tenantId, long requirementSpecificationId)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            return await dbContext.RequirementSpecifications
                .Include(x => x.Comments)
                .Where(x => x.TenantId == tenantId && x.Id == requirementSpecificationId).FirstOrDefaultAsync();
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
                foreach (var comment in dbContext.Comments.Where(x => x.RequirementId == requirement.Id))
                {
                    dbContext.Comments.Remove(comment);
                }

                dbContext.Requirements.Remove(requirement);
            }

            // Delete folders
            foreach (var folder in dbContext.RequirementSpecificationFolders.Where(x => x.RequirementSpecificationId == specification.Id))
            {
                dbContext.RequirementSpecificationFolders.Remove(folder);
            }

            // Delete comments
            foreach (var comment in dbContext.Comments.Where(x => x.RequirementSpecificationId == specification.Id))
            {
                dbContext.Comments.Remove(comment);
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
                foreach (var comment in dbContext.Comments.Where(x => x.RequirementId == requirement.Id))
                {
                    dbContext.Comments.Remove(comment);
                }
                dbContext.Requirements.Remove(requirement);
            }

            // Delete folders
            foreach (var folder in dbContext.RequirementSpecificationFolders.Where(x => x.RequirementSpecificationId == specification.Id))
            {
                dbContext.RequirementSpecificationFolders.Remove(folder);
            }
            foreach (var comment in dbContext.Comments.Where(x => x.RequirementSpecificationId == specification.Id))
            {
                dbContext.Comments.Remove(comment);
            }
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateRequirementSpecificationAsync(RequirementSpecification specification)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            if (string.IsNullOrEmpty(specification.Slug) && specification.TenantId is not null)
            {
                specification.Slug = await GenerateSpecificationSlugAsync(specification.TenantId, specification.Name);
            }

            dbContext.RequirementSpecifications.Update(specification);
            await dbContext.SaveChangesAsync();
        }
        public async Task AddRequirementAsync(Requirement requirement)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            await CalculatePathAsync(dbContext, requirement);
            await AssignExternalIdAsync(dbContext, requirement);

            if (string.IsNullOrEmpty(requirement.Slug) && requirement.TenantId is not null)
            {
                requirement.Slug = await GenerateRequirementSlugAsync(requirement.TenantId, requirement.Name);
            }

            if (requirement.ParentRequirementId is not null)
            {
                requirement.RootRequirementId = await FindUpstreamRootAsync(requirement.ParentRequirementId);
            }

            await dbContext.Requirements.AddAsync(requirement);
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateRequirementAsync(Requirement requirement)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            var existingRequirement = await dbContext.Requirements.AsNoTracking().Where(x => x.Id == requirement.Id).FirstOrDefaultAsync();
            if (existingRequirement is null)
            {
                throw new InvalidOperationException("Requirement does not exist!");
            }

            if (existingRequirement.RequirementType != requirement.RequirementType || existingRequirement.RequirementSpecificationId != requirement.RequirementSpecificationId || requirement.ExternalId is null)
            {
                await AssignExternalIdAsync(dbContext, requirement);
            }

            var hasSpecChanged = requirement.RequirementSpecificationId != existingRequirement.RequirementSpecificationId;
            var hasPathChanged = requirement.RequirementSpecificationFolderId != existingRequirement.RequirementSpecificationFolderId;
            if (hasPathChanged || hasSpecChanged)
            {
                await CalculatePathAsync(dbContext, requirement);
            }

            if(requirement.ParentRequirementId is not null)
            {
                requirement.RootRequirementId = await FindUpstreamRootAsync(requirement.ParentRequirementId);
            }

            if (string.IsNullOrEmpty(requirement.Slug) && requirement.TenantId is not null)
            {
                requirement.Slug = await GenerateRequirementSlugAsync(requirement.TenantId, requirement.Name);
            }

            dbContext.Requirements.Update(requirement);
            await dbContext.SaveChangesAsync();
        }

        private async Task<long?> FindUpstreamRootAsync(long? parentRequirementId)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            long? returnValue = parentRequirementId;
            while (parentRequirementId is not null)
            {
                var result = await dbContext.Requirements.Where(x => x.Id == parentRequirementId.Value).Select(x => new { x.Id, x.ParentRequirementId }).FirstOrDefaultAsync();
                if(result is null)
                {
                    break;
                }
                returnValue = result.Id;
                parentRequirementId = result.ParentRequirementId;
            }
            return returnValue;
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

        public async Task<Requirement?> GetRequirementByIdAsync(string tenantId, long id)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            return await dbContext.Requirements
                .Include(x=>x.RequirementFields)
                .Include(x=>x.Comments)
                .AsNoTracking().Where(x => x.TenantId == tenantId && x.Id == id).FirstOrDefaultAsync();
        }

        public async Task DeleteRequirementAsync(Requirement requirement)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            foreach (var link in dbContext.RequirementTestLinks.Where(x => x.RequirementId == requirement.Id))
            {
                dbContext.RequirementTestLinks.Remove(link);
            }
            foreach (var comment in dbContext.Comments.Where(x => x.RequirementId == requirement.Id))
            {
                dbContext.Comments.Remove(comment);
            }
            dbContext.Requirements.Remove(requirement);
            await dbContext.SaveChangesAsync();
        }

        public async Task<RequirementTestLink[]> SearchRequirementLinksAsync(FilterSpecification<RequirementTestLink>[] filters)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var requirements = dbContext.RequirementTestLinks
                .Include(x => x.Requirement)
                .Include(x => x.TestCase!).ThenInclude(y => y.TestCaseFields)
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

            if (folder.TestProjectId is null)
            {
                folder.TestProjectId = await dbContext.RequirementSpecifications.Where(x => x.Id == folder.RequirementSpecificationId).Select(x => x.TestProjectId).FirstOrDefaultAsync();
            }
            if (folder.TeamId is null)
            {
                folder.TeamId = await dbContext.RequirementSpecifications.Where(x => x.Id == folder.RequirementSpecificationId).Select(x => x.TeamId).FirstOrDefaultAsync();
            }
            await CalculatePathAsync(dbContext, folder);
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

            var hasSpecChanged = existingFolder.RequirementSpecificationId != folder.RequirementSpecificationId;
            var hasPathChanged = existingFolder.Name != folder.Name || existingFolder.ParentId != folder.ParentId;
            if (hasPathChanged || hasSpecChanged)
            {
                await CalculatePathAsync(dbContext, folder);
            }

            dbContext.RequirementSpecificationFolders.Update(folder);
            await dbContext.SaveChangesAsync();

            if (hasPathChanged || hasSpecChanged)
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

        /// <summary>
        /// Calculates the path and returns true if it has changed from before
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        private async Task<bool> CalculatePathAsync(ApplicationDbContext dbContext, Requirement requirement)
        {
            var pathBefore = requirement.Path;

            List<string> pathComponents = new();
            List<long> pathIds = new();
            await CalculatePathAsync(dbContext, requirement.RequirementSpecificationFolderId, pathComponents, pathIds);
            pathComponents.Reverse();
            pathIds.Reverse();
            requirement.Path = string.Join('/', pathComponents);
            requirement.PathIds = pathIds.ToArray();

            return pathBefore != requirement.Path;
        }

        private async Task<bool> CalculatePathAsync(ApplicationDbContext dbContext, RequirementSpecificationFolder folder)
        {
            var pathBefore = folder.Path;

            List<string> pathComponents = new();
            List<long> pathIds = new();
            await CalculatePathAsync(dbContext, folder.ParentId, pathComponents, pathIds);
            pathComponents.Reverse();
            pathIds.Reverse();
            folder.Path = string.Join('/', pathComponents);
            folder.PathIds = pathIds.ToArray();

            return pathBefore != folder.Path;
        }


        #endregion Path

        #region Slugs
        private async Task<string> GenerateRequirementSlugAsync(string tenantId, string name)
        {
            var slugHelper = new Slugify.SlugHelper();
            var slug = slugHelper.GenerateSlug(name);
            int counter = 1;
            while (await RequirementSlugExistsAsync(tenantId, slug))
            {
                slug = slugHelper.GenerateSlug(slug + counter);
                counter++;
            }
            return slug;
        }
        private async Task<bool> RequirementSlugExistsAsync(string tenantId, string slug)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            return await dbContext.Requirements.AsNoTracking().Where(x => x.Slug == slug && x.TenantId == tenantId).AnyAsync();
        }
        private async Task<string> GenerateSpecificationSlugAsync(string tenantId, string name)
        {
            var slugHelper = new Slugify.SlugHelper();
            var slug = slugHelper.GenerateSlug(name);
            int counter = 1;
            while (await SpecificationSlugExistsAsync(tenantId, slug))
            {
                slug = slugHelper.GenerateSlug(slug + counter);
                counter++;
            }
            return slug;
        }
        private async Task<bool> SpecificationSlugExistsAsync(string tenantId, string slug)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            return await dbContext.RequirementSpecifications.AsNoTracking().Where(x => x.Slug == slug && x.TenantId == tenantId).AnyAsync();
        }
        #endregion
    }
}
