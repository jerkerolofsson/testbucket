using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.TestRuns;
using TestBucket.Domain.Testing.TestRuns.Search;

namespace TestBucket.Data.Testing;

internal class TestRunInsightsRepository : ITestRunInsightsRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public TestRunInsightsRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<double> GetCodeCoverageAsync(long testRunId)
    {
        var name = testRunId.ToString();

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var group = await dbContext.CodeCoverageGroups
            .Where(x=>x.Group == Domain.Code.CodeCoverage.Models.CodeCoverageGroupType.TestRun && x.Name == name) 
            .FirstOrDefaultAsync();

        if(group is not null)
        {
            return group.CoveredLineCount * 100.0 / group.LineCount; 
        }

        return 0;
    }
}
