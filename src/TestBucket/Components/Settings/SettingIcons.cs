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
                return Icons.Material.Outlined.Palette;

            case SettingIcon.Server:
                return Icons.Material.Outlined.Computer;

            case SettingIcon.AI:
                return Icons.Material.Outlined.Rocket;

            case SettingIcon.Accessibility:
                return Icons.Material.Outlined.AccessibilityNew;
        }

        return Icons.Material.Outlined.Settings;
    }
}
