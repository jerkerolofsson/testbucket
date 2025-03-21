using System.Linq.Expressions;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Specifications;

public class FilterTestCasesByText : FilterSpecification<TestCase>
{
    private readonly string? _query;

    public FilterTestCasesByText(string searchPhrase)
    {
        _query = searchPhrase.ToLower();
    }

    protected override Expression<Func<TestCase, bool>> GetExpression()
    {
        return x => x.Name.ToLower().Contains(_query);
    }
}
