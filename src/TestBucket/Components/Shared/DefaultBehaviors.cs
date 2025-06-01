namespace TestBucket.Components.Shared;

public static class DefaultBehaviors
{
    public static DialogOptions DialogOptions => new DialogOptions
    {
        BackgroundClass = "tb-blur-overlay",
        BackdropClick = true,
        CloseOnEscapeKey = true,
        CloseButton = true,
    };
}
