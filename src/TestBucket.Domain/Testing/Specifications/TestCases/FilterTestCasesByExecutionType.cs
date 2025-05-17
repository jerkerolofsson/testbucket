using System.Linq.Expressions;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Specifications.TestCases;

public class FilterTestCasesByExecutionType : FilterSpecification<TestCase>
{
    private readonly TestExecutionType _value;

    public FilterTestCasesByExecutionType(TestExecutionType type)
    {
        _value = type;
    }

    protected override Expression<Func<TestCase, bool>> GetExpression()
    {
        return x => x.ExecutionType == _value;
    }
}
