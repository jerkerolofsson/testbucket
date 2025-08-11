using System.Linq.Expressions;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Specifications.TestCases;

public class FilterTestCasesByBooleanField : FilterSpecification<TestCase>
{
    private readonly long _fieldDefinitionId;
    private readonly bool _value;

    public FilterTestCasesByBooleanField(long fieldDefinitionId, bool value)
    {
        _fieldDefinitionId = fieldDefinitionId;
        _value = value;
    }

    protected override Expression<Func<TestCase, bool>> GetExpression()
    {
        if (_value)
        {
            return x => x.TestCaseFields!.Where(f => f.FieldDefinitionId == _fieldDefinitionId && f.BooleanValue == true).Any();
        }
        else
        {
            // false or missing field
            return x => !x.TestCaseFields!.Where(f => f.FieldDefinitionId == _fieldDefinitionId && f.BooleanValue == true).Any();
        }
    }
}
