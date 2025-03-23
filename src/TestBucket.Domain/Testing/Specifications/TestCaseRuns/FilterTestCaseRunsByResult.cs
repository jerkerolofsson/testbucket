using System.Linq.Expressions;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Specifications.TestCaseRuns;

public class FilterTestCaseRunsByResult : FilterSpecification<TestCaseRun>
{
    private readonly TestResult _result;

    public FilterTestCaseRunsByResult(TestResult result)
    {
        _result = result;
    }

    protected override Expression<Func<TestCaseRun, bool>> GetExpression()
    {
        return x => x.Result == _result;
    }
}
