using TestBucket.Traits.Core;

namespace TestBucket.Domain.Issues.Models;
public static class LocalIssueExtensions
{
    public static IReadOnlyList<string> GetLabels(this LocalIssue issue)
    {
        var field = issue.GetFieldByTrait(TraitType.Label);
        if(field is null)
        {
            return [];
        }

        if(field.StringValuesList is not null)
        {
            return field.StringValuesList;
        }

        if(field.StringValue is not null)
        {
            return [field.StringValue];
        }

        return [];
    }

    public static IssueField? GetFieldByTrait(this LocalIssue issue, TraitType type)
    {

        return issue.IssueFields?.FirstOrDefault(x=>x.FieldDefinition?.TraitType == type);
    }
}
