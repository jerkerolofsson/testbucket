using System.Linq.Expressions;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Specifications.TestCaseRuns;

public class FilterTestCaseRunsIncomplete : FilterSpecification<TestCaseRun>
{
    public FilterTestCaseRunsIncomplete()
    {
    }

    protected override Expression<Func<TestCaseRun, bool>> GetExpression()
    {
        return x => x.Result == TestResult.NoRun;
    }
}
