using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Testing.Models;
using TestBucket.Domain.Insights.Model;
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


    public async Task<InsightsData<DateOnly, double>> GetCodeCoverageTrendAsync(IEnumerable<FilterSpecification<TestRun>> filters)
    {

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var runs = dbContext.TestRuns
            .Include(x => x.TestRunFields)
            .AsSplitQuery()
            .AsQueryable();

        foreach (var filter in filters)
        {
            runs = runs.Where(filter.Expression);
        }

        var runIds = await runs.Select(x => x.Id.ToString()).ToListAsync();

        var rows = dbContext.CodeCoverageGroups
            .Where(x => x.Group == Domain.Code.CodeCoverage.Models.CodeCoverageGroupType.TestRun && runIds.Contains(x.Name))
            .OrderBy(x=>x.Created);

        var data = new InsightsData<DateOnly, double>();
        var series = data.Add("Code Coverage");
        await foreach(var group in rows.AsAsyncEnumerable())
        {
            if (group.LineCount > 0)
            {
                double percent = group.CoveredLineCount * 100.0 / group.LineCount;

                var date = new DateOnly(group.Created.Year, group.Created.Month, group.Created.Day);
                if (series.TryGetValue(date, out var existing))
                {
                    series[date] = Math.Max(percent, existing);
                }
                else
                {
                    series.Add(date, percent);
                }
            }
        }

        return data;
    }


    public async Task<double> GetCodeCoverageAsync(IEnumerable<FilterSpecification<TestRun>> filters)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var runs = dbContext.TestRuns
          .Include(x => x.TestRunFields)
          .AsSplitQuery()
          .AsQueryable();

        foreach (var filter in filters)
        {
            runs = runs.Where(filter.Expression);
        }

        // Get the latest run matching the filter
        var testRun = await runs.OrderByDescending(x => x.Created).FirstOrDefaultAsync();

        if (testRun is not null)
        {
            var name = testRun.Id.ToString();

            var group = await dbContext.CodeCoverageGroups
                .Where(x => x.Group == Domain.Code.CodeCoverage.Models.CodeCoverageGroupType.TestRun && x.Name == name)
                .FirstOrDefaultAsync();

            if (group is not null && group.LineCount > 0)
            {
                return group.CoveredLineCount * 100.0 / group.LineCount;
            }
        }

        return 0;
    }
}
