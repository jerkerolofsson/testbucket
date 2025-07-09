namespace TestBucket.Contracts.Keyboard;

[Flags]
public enum ModifierKey
{
    None = 0,
    Shift = 1,
    Ctrl = 2,
    Alt = 4,
    Meta = 8
}
