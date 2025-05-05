using System.Linq.Expressions;

using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Requirements.Specifications.Requirements;

public class FilterRequirementByStringField : FilterSpecification<Requirement>
{
    private readonly long _fieldDefinitionId;
    private readonly string? _categeory;

    public FilterRequirementByStringField(long fieldDefinitionId, string categeory)
    {
        _fieldDefinitionId = fieldDefinitionId;
        _categeory = categeory;
    }

    protected override Expression<Func<Requirement, bool>> GetExpression()
    {
        return x => x.RequirementFields!.Where(f => f.FieldDefinitionId == _fieldDefinitionId && f.StringValue == _categeory).Any();
    }
}
