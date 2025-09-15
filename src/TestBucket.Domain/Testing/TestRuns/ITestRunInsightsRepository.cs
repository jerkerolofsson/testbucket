using TestBucket.Domain.Insights.Model;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.TestRuns;

public interface ITestRunInsightsRepository
{
    /// <summary>
    /// Returns code coverage, in percentage, for the given query
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="testRunId"></param>
    /// <returns></returns>
    Task<double> GetCodeCoverageAsync(IEnumerable<FilterSpecification<TestRun>> filters);
    Task<InsightsData<DateOnly, double>> GetCodeCoverageTrendAsync(IEnumerable<FilterSpecification<TestRun>> filters);
}
