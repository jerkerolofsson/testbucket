using TestBucket.Domain.Code.CodeCoverage;
using TestBucket.Domain.Code.CodeCoverage.Models;

namespace TestBucket.Data.Code;

public class CodeCoverageRepository : ICodeCoverageRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public CodeCoverageRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task AddGroupAsync(CodeCoverageGroup group)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.CodeCoverageGroups.Add(group);
        await dbContext.SaveChangesAsync();
    }

    public async Task<CodeCoverageGroup?> GetGroupAsync(string tenantId, long projectId, CodeCoverageGroupType groupType, string groupName)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.CodeCoverageGroups
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.TestProjectId == projectId && x.Group == groupType && x.Name == groupName)
            .FirstOrDefaultAsync();
    }

    public async Task UpdateGroupAsync(CodeCoverageGroup group)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.CodeCoverageGroups.Update(group);
        await dbContext.SaveChangesAsync();
    }
}
