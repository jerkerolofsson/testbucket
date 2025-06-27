using MudGantt;

using TestBucket.Domain.Requirements.Models;

namespace TestBucket.Components.Requirements.Utilities;

internal class RequirementGanntHelper
{

    public static Link[] GetDependencies(Requirement requirement)
    {
        if (requirement.ParentRequirementId is not null)
        {
            return [new Link(requirement.ParentRequirementId.Value.ToString(), LinkType.StartToStart)];
        }
        return [];
    }
}
