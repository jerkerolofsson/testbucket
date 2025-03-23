using System.Linq.Expressions;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Specifications.TestCases;

public class FilterTestCasesByTestSuite : FilterSpecification<TestCase>
{
    private readonly long _id;

    public FilterTestCasesByTestSuite(long id)
    {
        _id = id;
    }

    protected override Expression<Func<TestCase, bool>> GetExpression()
    {
        return x => x.TestSuiteId == _id;
    }
}
