using TestBucket.Domain.AI.Mcp.Models;

namespace TestBucket.Domain.AI.Mcp.Helpers;
internal class McpToolAccessChecker
{
    public static bool HasAccess(long? projectId, ClaimsPrincipal user, McpServerRegistration registration)
    {
        var userName = user.Identity?.Name;
        if(userName is null)
        {
            return false;
        }
        if (registration.TestProjectId != projectId && registration.TestProjectId is not null)
        {
            return false;
        }
        if (userName == registration.CreatedBy || registration.PublicForProject)
        {
            return true;
        }
        return false;
    }
}
