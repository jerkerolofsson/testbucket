using TestBucket.Domain;
using TestBucket.Domain.Settings.Models;

namespace TestBucket.Components.Settings;

public static class SettingIcons
{
    public static string GetIcon(SettingIcon icon)
    {
        switch(icon)
        {
            case SettingIcon.None:
                return "";

            case SettingIcon.Appearance:
                return TbIcons.BoldDuoTone.Palette;

            case SettingIcon.Profile:
                return Icons.Material.Outlined.VerifiedUser;

            case SettingIcon.Server:
                return Icons.Material.Outlined.Computer;

            case SettingIcon.AI:
                return TbIcons.BoldDuoTone.AI;

            case SettingIcon.Accessibility:
                return TbIcons.BoldDuoTone.Accessibility;
        }

        return Icons.Material.Outlined.Settings;
    }
}
