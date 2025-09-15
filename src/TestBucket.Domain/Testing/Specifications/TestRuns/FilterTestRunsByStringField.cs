using System.Linq.Expressions;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Specifications.TestRuns;

public class FilterTestRunsByStringField : FilterSpecification<TestRun>
{
    private readonly long _fieldDefinitionId;
    private readonly string? _categeory;

    public FilterTestRunsByStringField(long fieldDefinitionId, string categeory)
    {
        _fieldDefinitionId = fieldDefinitionId;
        _categeory = categeory;
    }

    protected override Expression<Func<TestRun, bool>> GetExpression()
    {
        if(_categeory == "null")
        {
            return x => !x.TestRunFields!.Where(f => f.FieldDefinitionId == _fieldDefinitionId && (f.StringValue != null && f.StringValue.Length > 0)).Any();
        }
        return x => x.TestRunFields!.Where(f => f.FieldDefinitionId == _fieldDefinitionId && f.StringValue == _categeory).Any();
    }
}
