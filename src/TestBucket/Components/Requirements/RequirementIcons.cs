using TestBucket.Domain;
using TestBucket.Domain.Requirements.Models;

namespace TestBucket.Components.Requirements;

public class RequirementIcons
{

    public static string GetIcon(Requirement requirement)
    {
        return TbIcons.BoldDuoTone.Medal;
    }

    public static string GetIcon(RequirementSpecification specification)
    {
        return specification.Icon ?? Icons.Material.Outlined.Article;
    }

    public static string GetIcon(RequirementSpecificationFolder x)
    {
        return x.Icon ?? TbIcons.BoldOutline.Folder;
    }
}
