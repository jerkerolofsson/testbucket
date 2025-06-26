using TestBucket.Contracts.Requirements.Types;
using TestBucket.Domain;
using TestBucket.Domain.Requirements.Models;

namespace TestBucket.Components.Requirements;

public class RequirementColors
{
    public static string GetColor(Requirement requirement)
    {
        if (requirement.RequirementType is not null)
        {
            switch (requirement.RequirementType)
            {
                case RequirementTypes.Task:
                    return "#019356";
                case RequirementTypes.Initiative:
                    return "#D31176";
                case RequirementTypes.Epic:
                    return "#D31176";
            }
        }
        return "#1176D3"; // blue
    }
}
