using TestBucket.Domain.Requirements.Models;
using TestBucket.Traits.Core;

namespace TestBucket.Components.Tests.TestRuns.Extensions;

public static class TestRunExtensions
{
    public static string? GetMilestone(this TestRun requirement)
    {
        if (requirement.TestRunFields is not null)
        {
            foreach (var field in requirement.TestRunFields)
            {
                if (field.FieldDefinition?.TraitType == TraitType.Milestone)
                {
                    return field.GetValueAsString();
                }
            }
        }
        return null;
    }
}
