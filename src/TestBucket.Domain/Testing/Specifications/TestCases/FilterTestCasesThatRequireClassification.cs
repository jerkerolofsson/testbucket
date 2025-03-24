using System.Linq.Expressions;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Specifications.TestCases;

public class FilterTestCasesThatRequireClassification : FilterSpecification<TestCase>
{
    public FilterTestCasesThatRequireClassification()
    {
    }

    protected override Expression<Func<TestCase, bool>> GetExpression()
    {
        return x => x.ClassificationRequired;
    }
}
