using System.Linq.Expressions;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Specifications.TestCases;

public class FilterTestCasesByExternalDisplayId : FilterSpecification<TestCase>
{
    private readonly string _query;

    public FilterTestCasesByExternalDisplayId(string id)
    {
        _query = id;
    }

    protected override Expression<Func<TestCase, bool>> GetExpression()
    {
        return x => x.ExternalDisplayId == _query;
    }
}
