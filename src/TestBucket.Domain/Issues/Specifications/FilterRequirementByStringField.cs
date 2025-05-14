using System.Linq.Expressions;

using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Issues.Specifications;

public class FilterLocalIssueByStringField : FilterSpecification<LocalIssue>
{
    private readonly long _fieldDefinitionId;
    private readonly string? _categeory;

    public FilterLocalIssueByStringField(long fieldDefinitionId, string categeory)
    {
        _fieldDefinitionId = fieldDefinitionId;
        _categeory = categeory;
    }

    protected override Expression<Func<LocalIssue, bool>> GetExpression()
    {
        return x => x.IssueFields!.Where(f => f.FieldDefinitionId == _fieldDefinitionId && f.StringValue == _categeory).Any();
    }
}
