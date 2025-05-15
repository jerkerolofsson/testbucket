using System.Linq.Expressions;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Specifications.TestCases;

public class FilterTestCaseRunsByBooleanField : FilterSpecification<TestCaseRun>
{
    private readonly long _fieldDefinitionId;
    private readonly bool _value;

    public FilterTestCaseRunsByBooleanField(long fieldDefinitionId, bool value)
    {
        _fieldDefinitionId = fieldDefinitionId;
        _value = value;
    }

    protected override Expression<Func<TestCaseRun, bool>> GetExpression()
    {
        return x => x.TestCaseRunFields!.Where(f => f.FieldDefinitionId == _fieldDefinitionId && f.BooleanValue == _value).Any();
    }
}
