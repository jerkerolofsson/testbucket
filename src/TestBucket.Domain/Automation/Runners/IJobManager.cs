
using TestBucket.Domain.Automation.Runners.Models;

namespace TestBucket.Domain.Automation.Runners;
public interface IJobManager
{
    Task AddAsync(ClaimsPrincipal principal, Job job);
    Task<Job?> GetByIdAsync(ClaimsPrincipal principal, long id);
    Task<Job?> GetJobByGuidAsync(ClaimsPrincipal principal, string guid);
}