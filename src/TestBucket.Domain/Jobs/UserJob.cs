using Quartz;

using TestBucket.Domain.Identity;

namespace TestBucket.Domain.Jobs;

/// <summary>
/// Base job
/// </summary>
public abstract class UserJob : IJob
{
    public abstract Task Execute(IJobExecutionContext context);

    /// <summary>
    /// Returns an impersonated user with full access
    /// 
    /// Access must be checked before creating a job!
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedAccessException"></exception>
    protected ClaimsPrincipal GetUser(IJobExecutionContext context)
    {
        var tenantId = context.MergedJobDataMap.GetString("TenantId");
        if (tenantId is null)
        {
            throw new UnauthorizedAccessException("Error, no tenant ID specified in job");
        }
        var userName = context.MergedJobDataMap.GetString("UserName");
        if (userName is null)
        {
            throw new UnauthorizedAccessException("Error, username not specified in the job");
        }

        // Note: Access is checked before creating a job
        var principal = Impersonation.ImpersonateUserWithFullAccess(tenantId, userName);
        return principal;
    }
}
