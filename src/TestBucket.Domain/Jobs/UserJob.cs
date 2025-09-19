using System.Security.Claims;

using Quartz;

using TestBucket.Domain.Identity;
using TestBucket.Domain.Identity.Permissions.Models;

namespace TestBucket.Domain.Jobs;

/// <summary>
/// Base job
/// </summary>
public abstract class UserJob : IJob
{
    public const string KEY_USERNAME = "UserName";
    public const string KEY_TENANT_ID = "TenantId";
    public const string KEY_PERMSISSIONS = "Permissions";

    public abstract Task Execute(IJobExecutionContext context);

    internal static ClaimsPrincipal CreateClaimsPrincipal(JobDataMap map, string tenantId, string userName)
    {

        var permissionsSerialized = map.GetString(UserJob.KEY_PERMSISSIONS);
        if (permissionsSerialized is null)
        {
            throw new KeyNotFoundException($"Missing mandatory key: {KEY_PERMSISSIONS}");
        }

        UserPermissions permissions = PermissionClaimSerializer.Deserialize(permissionsSerialized);

        var principal = Impersonation.ImpersonateUser(tenantId, userName, permissions);
        return principal;
    }

    /// <summary>
    /// Returns an impersonated user with full access
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedAccessException"></exception>
    protected ClaimsPrincipal GetUser(IJobExecutionContext context)
    {
        var map = context.MergedJobDataMap;
        return GetUser(map);

    }

    public static ClaimsPrincipal GetUser(JobDataMap map)
    {
        var tenantId = map.GetString(UserJob.KEY_TENANT_ID);
        if (tenantId is null)
        {
            throw new KeyNotFoundException($"Missing mandatory key: {KEY_TENANT_ID}");
        }
        var userName = map.GetString(UserJob.KEY_USERNAME);
        if (userName is null)
        {
            throw new KeyNotFoundException($"Missing mandatory key: {KEY_USERNAME}");
        }

        return CreateClaimsPrincipal(map, tenantId, userName);
    }
}
