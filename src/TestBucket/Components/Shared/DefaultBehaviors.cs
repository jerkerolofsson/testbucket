namespace TestBucket.Components.Shared;

public static class DefaultBehaviors
{
    public static DialogOptions DialogOptions => new DialogOptions
    {
        BackdropClick = true,
        CloseOnEscapeKey = true,
        CloseButton = true
    };
}
