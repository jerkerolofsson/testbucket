using System.Linq.Expressions;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Specifications.TestCaseRuns;

public class FilterTestCaseRunsByText : FilterSpecification<TestCaseRun>
{
    private readonly string? _query;

    public FilterTestCaseRunsByText(string searchPhrase)
    {
        _query = searchPhrase.ToLower();
    }

    protected override Expression<Func<TestCaseRun, bool>> GetExpression()
    {
        return x => x.Name.ToLower().Contains(_query);
    }
}
