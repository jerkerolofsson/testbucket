using System.Linq.Expressions;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Specifications.TestCases;

public class FilterTestCasesByStringField : FilterSpecification<TestCase>
{
    private readonly long _fieldDefinitionId;
    private readonly string? _categeory;

    public FilterTestCasesByStringField(long fieldDefinitionId, string categeory)
    {
        _fieldDefinitionId = fieldDefinitionId;
        _categeory = categeory;
    }

    protected override Expression<Func<TestCase, bool>> GetExpression()
    {
        if(_categeory == "null")
        {
            return x => !x.TestCaseFields!.Where(f => f.FieldDefinitionId == _fieldDefinitionId && (f.StringValue != null && f.StringValue.Length > 0)).Any();
        }
        return x => x.TestCaseFields!.Where(f => f.FieldDefinitionId == _fieldDefinitionId && f.StringValue == _categeory).Any();
    }
}
