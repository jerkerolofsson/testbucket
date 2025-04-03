using System.Linq.Expressions;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Specifications.TestCaseRuns;

public class FilterTestCaseRunsCompleted : FilterSpecification<TestCaseRun>
{
    public FilterTestCaseRunsCompleted()
    {
    }

    protected override Expression<Func<TestCaseRun, bool>> GetExpression()
    {
        return x => x.Result != TestResult.NoRun;
    }
}
