namespace TestBucket.Domain.AI;
public class TimelineAiIconHelper
{
    public static string GetTimelineIcon(ClaimsPrincipal principal, string defaultIcon)
    {
        string icon = defaultIcon;
        if (principal.Identity?.Name is not null)
        {
            var model = LlmModels.GetModelByName(principal.Identity.Name);
            if (model?.Icon is not null)
            {
                icon = model.Icon;
            }
        }
        return icon;
    }
}
