using System.Linq.Expressions;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Specifications.TestCases;

public class FilterTestCasesExcludeAutomated : FilterSpecification<TestCase>
{

    protected override Expression<Func<TestCase, bool>> GetExpression()
    {
        return x => x.ExecutionType != TestExecutionType.Automated;
    }
}
