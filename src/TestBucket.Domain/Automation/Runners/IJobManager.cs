
using TestBucket.Domain.Automation.Runners.Models;

namespace TestBucket.Domain.Automation.Runners;
public interface IJobManager
{
    Task<Job?> GetJobByGuidAsync(ClaimsPrincipal principal, string guid);
}