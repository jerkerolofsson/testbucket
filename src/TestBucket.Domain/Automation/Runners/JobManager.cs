
using TestBucket.Domain.Automation.Runners.Models;

namespace TestBucket.Domain.Automation.Runners;
internal class JobManager :IJobManager
{
    private readonly IJobRepository _repository;

    public JobManager(IJobRepository repository)
    {
        _repository = repository;
    }

    public async Task<Job?> GetJobByGuidAsync(ClaimsPrincipal principal, string guid)
    {
        var job = await _repository.GetByGuidAsync(guid);
        if (job is not null)
        {
            principal.ThrowIfEntityTenantIsDifferent(job);
        }
        return job;
    }
}