using System.Linq.Expressions;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Specifications.TestRuns;

public class FilterTestRunsByBooleanField : FilterSpecification<TestRun>
{
    private readonly long _fieldDefinitionId;
    private readonly bool _value;

    public FilterTestRunsByBooleanField(long fieldDefinitionId, bool value)
    {
        _fieldDefinitionId = fieldDefinitionId;
        _value = value;
    }

    protected override Expression<Func<TestRun, bool>> GetExpression()
    {
        if (_value)
        {
            return x => x.TestRunFields!.Where(f => f.FieldDefinitionId == _fieldDefinitionId && f.BooleanValue == true).Any();
        }
        else
        {
            // false or missing field
            return x => !x.TestRunFields!.Where(f => f.FieldDefinitionId == _fieldDefinitionId && f.BooleanValue == true).Any();
        }
    }
}
