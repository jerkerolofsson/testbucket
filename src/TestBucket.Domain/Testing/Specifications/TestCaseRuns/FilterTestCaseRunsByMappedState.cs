using System.Linq.Expressions;

using TestBucket.Contracts.Testing.States;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Specifications.TestCaseRuns;

public class FilterTestCaseRunsByMappedState : FilterSpecification<TestCaseRun>
{
    private readonly MappedTestState _state;

    public FilterTestCaseRunsByMappedState(MappedTestState state)
    {
        _state = state;
    }

    protected override Expression<Func<TestCaseRun, bool>> GetExpression()
    {
        return x => x.MappedState == _state;
    }
}
