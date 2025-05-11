using System.Linq.Expressions;

using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Requirements.Specifications.Requirements;

public class FilterRequirementByBooleanField : FilterSpecification<Requirement>
{
    private readonly long _fieldDefinitionId;
    private readonly bool _value;

    public FilterRequirementByBooleanField(long fieldDefinitionId, bool value)
    {
        _fieldDefinitionId = fieldDefinitionId;
        _value = value;
    }

    protected override Expression<Func<Requirement, bool>> GetExpression()
    {
        return x => x.RequirementFields!.Where(f => f.FieldDefinitionId == _fieldDefinitionId && f.BooleanValue == _value).Any();
    }
}
