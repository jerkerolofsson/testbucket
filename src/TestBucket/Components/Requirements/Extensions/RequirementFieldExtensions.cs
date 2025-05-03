using TestBucket.Domain.Requirements.Models;
using TestBucket.Traits.Core;

namespace TestBucket.Components.Requirements.Extensions;

internal static class RequirementFieldExtensions
{
    public static string? GetMilestone(this Requirement requirement)
    {
        if(requirement.RequirementFields is not null)
        {
            foreach(var field in requirement.RequirementFields)
            {
                if(field.FieldDefinition?.TraitType == TraitType.Milestone)
                {
                    return field.GetValueAsString();
                }
            }
        }
        return null;
    }
    public static string? GetFeature(this Requirement requirement)
    {
        if (requirement.RequirementFields is not null)
        {
            foreach (var field in requirement.RequirementFields)
            {
                if (field.FieldDefinition?.TraitType == TraitType.Feature)
                {
                    return field.GetValueAsString();
                }
            }
        }
        return null;
    }
}
