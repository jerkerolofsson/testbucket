using System.Linq.Expressions;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Specifications.TestCases;

public class FilterTestCasesById : FilterSpecification<TestCase>
{
    private readonly long _query;

    public FilterTestCasesById(long id)
    {
        _query = id;
    }

    protected override Expression<Func<TestCase, bool>> GetExpression()
    {
        return x => x.Id == _query;
    }
}
