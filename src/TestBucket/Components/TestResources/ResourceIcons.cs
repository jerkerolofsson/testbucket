using TestBucket.Domain;
using TestBucket.Domain.TestResources.Models;

namespace TestBucket.Components.TestResources;

public static class ResourceIcons
{
    public static string GetResourceIcon(string resourceType)
    {
        if(resourceType is not null)
        {
            if (resourceType.Contains("android"))
            {
                return MudBlazor.Icons.Material.Filled.Android;
            }
            if (resourceType.Contains("phone"))
            {
                return TbIcons.BoldDuoTone.SmartPhone;
            }
        }

        return TbIcons.BoldDuoTone.Laptop;
    }
}
