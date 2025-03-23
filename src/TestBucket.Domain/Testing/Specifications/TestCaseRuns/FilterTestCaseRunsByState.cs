using System.Linq.Expressions;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Specifications.TestCaseRuns;

public class FilterTestCaseRunsByState : FilterSpecification<TestCaseRun>
{
    private readonly string _state;

    public FilterTestCaseRunsByState(string state)
    {
        _state = state;
    }

    protected override Expression<Func<TestCaseRun, bool>> GetExpression()
    {
        return x => x.State == _state;
    }
}
