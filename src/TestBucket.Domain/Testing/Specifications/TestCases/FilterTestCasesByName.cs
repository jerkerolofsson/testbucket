using System.Linq.Expressions;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Specifications.TestCases;

public class FilterTestCasesByName : FilterSpecification<TestCase>
{
    private readonly string _query;

    public FilterTestCasesByName(string name)
    {
        _query = name;
    }

    protected override Expression<Func<TestCase, bool>> GetExpression()
    {
        return x => x.Name == _query;
    }
}
