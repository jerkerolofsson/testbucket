
using TestBucket.Domain.Automation.Runners.Models;

namespace TestBucket.Domain.Automation.Runners;
internal class JobManager :IJobManager
{
    private readonly IJobRepository _repository;
    private readonly TimeProvider _timeProvider;

    public JobManager(IJobRepository repository, TimeProvider timeProvider)
    {
        _repository = repository;
        _timeProvider = timeProvider;
    }

    public async Task<Job?> GetByIdAsync(ClaimsPrincipal principal, long id)
    {
        ArgumentNullException.ThrowIfNull(principal);
        principal.ThrowIfNoPermission(PermissionEntityType.TestRun, PermissionLevel.Read);

        var job = await _repository.GetByIdAsync(id);
        if (job is not null)
        {
            principal.ThrowIfEntityTenantIsDifferent(job);
        }
        return job;
    }

    public async Task AddAsync(ClaimsPrincipal principal, Job job)
    {
        ArgumentNullException.ThrowIfNull(principal);
        ArgumentNullException.ThrowIfNull(job);

        principal.ThrowIfNoPermission(PermissionEntityType.TestRun, PermissionLevel.Write);
        job.CreatedBy = job.ModifiedBy = principal.Identity?.Name ?? throw new UnauthorizedAccessException();
        job.Created = job.Modified = _timeProvider.GetUtcNow();
        job.TenantId = principal.GetTenantIdOrThrow();

        await _repository.AddAsync(job);
    }

    public async Task<Job?> GetJobByGuidAsync(ClaimsPrincipal principal, string guid)
    {
        ArgumentNullException.ThrowIfNull(principal);
        ArgumentNullException.ThrowIfNull(guid);
        principal.ThrowIfNoPermission(PermissionEntityType.TestRun, PermissionLevel.Read);

        var job = await _repository.GetByGuidAsync(guid);
        if (job is not null)
        {
            principal.ThrowIfEntityTenantIsDifferent(job);
        }
        return job;
    }
}