using TestBucket.Contracts.Requirements.Types;
using TestBucket.Domain;
using TestBucket.Domain.Requirements.Models;

namespace TestBucket.Components.Requirements;

public class RequirementIcons
{

    public static string GetIcon(Requirement requirement)
    {
        if (requirement.RequirementType is not null)
        {
            switch (requirement.RequirementType)
            {
                case RequirementTypes.Task:
                    return TbIcons.BoldDuoTone.CheckList;
                case RequirementTypes.Initiative:
                    return TbIcons.BoldDuoTone.Initiative;
                case RequirementTypes.Epic:
                    return TbIcons.BoldDuoTone.Epic;
                case RequirementTypes.Story:
                    return TbIcons.BoldDuoTone.Book;
            }
        }
        return TbIcons.BoldDuoTone.Medal;
    }

    public static string GetIcon(RequirementSpecification specification)
    {
        return specification.Icon ?? TbIcons.BoldDuoTone.Box;
    }

    public static string GetIcon(RequirementSpecificationFolder x)
    {
        return x.Icon ?? TbIcons.BoldOutline.Folder;
    }
}
