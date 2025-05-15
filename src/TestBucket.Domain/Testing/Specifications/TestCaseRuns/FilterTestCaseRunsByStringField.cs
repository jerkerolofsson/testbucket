using System.Linq.Expressions;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Specifications.TestCases;

public class FilterTestCaseRunsByStringField : FilterSpecification<TestCaseRun>
{
    private readonly long _fieldDefinitionId;
    private readonly string? _categeory;

    public FilterTestCaseRunsByStringField(long fieldDefinitionId, string categeory)
    {
        _fieldDefinitionId = fieldDefinitionId;
        _categeory = categeory;
    }

    protected override Expression<Func<TestCaseRun, bool>> GetExpression()
    {
        return x => x.TestCaseRunFields!.Where(f => f.FieldDefinitionId == _fieldDefinitionId && f.StringValue == _categeory).Any();
    }
}
