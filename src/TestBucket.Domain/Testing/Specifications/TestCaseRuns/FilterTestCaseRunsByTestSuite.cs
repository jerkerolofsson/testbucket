using System.Linq.Expressions;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Specifications.TestCaseRuns;

public class FilterTestCaseRunsByTestSuite : FilterSpecification<TestCaseRun>
{
    private readonly long _id;

    public FilterTestCaseRunsByTestSuite(long id)
    {
        _id = id;
    }

    protected override Expression<Func<TestCaseRun, bool>> GetExpression()
    {
        return x => x.TestCase != null && x.TestCase.TestSuiteId == _id;
    }
}
