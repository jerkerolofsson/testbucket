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
        return field.StringValuesList ?? [field.StringValue];
    }

    public static string GetLabelColor(this LocalIssue issue, string label)
    {
        var field = issue.GetFieldByTrait(TraitType.Label);
        if (field is not null)
        {
            if(field.FieldDefinition is not null)
            {
                if(field.FieldDefinition.OptionColors is not null && field.FieldDefinition.OptionColors.TryGetValue(label, out var color))
                {
                    return color;
                }
            }
        }
        return "#707070";
    }

    public static IssueField? GetFieldByTrait(this LocalIssue issue, TraitType type)
    {

        return issue.IssueFields?.FirstOrDefault(x=>x.FieldDefinition?.TraitType == type);
    }
}
