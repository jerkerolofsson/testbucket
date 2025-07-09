using TestBucket.Contracts.Keyboard;

using Toolbelt.Blazor.HotKeys2;

namespace TestBucket.Components.Layout.Controls;

public class HotKeysService
{
    public HotKeysContext? Context { get; internal set; }

    public static ModCode MapModCode(ModifierKey keys)
    {
        ModCode code = ModCode.None;
        if ((keys & ModifierKey.Ctrl) == ModifierKey.Ctrl)
        {
            code |= ModCode.Ctrl;
        }
        if ((keys & ModifierKey.Shift) == ModifierKey.Shift)
        {
            code |= ModCode.Shift;
        }
        if ((keys & ModifierKey.Meta) == ModifierKey.Meta)
        {
            code |= ModCode.Meta;
        }
        if ((keys & ModifierKey.Alt) == ModifierKey.Alt)
        {
            code |= ModCode.Alt;
        }
        return code;
    }
}
