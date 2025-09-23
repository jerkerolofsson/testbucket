using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TestBucket.Contracts.Automation;
using TestBucket.Domain.Automation.Runners;
using TestBucket.Domain.Automation.Runners.Models;

namespace TestBucket.Domain.UnitTests.Automation.Fakes;

internal class FakeJobRepository : IJobRepository
{
    private readonly List<Job> _jobs = new();

    public Task AddAsync(Job job)
    {
        if (job == null) throw new ArgumentNullException(nameof(job));
        // Simulate auto-increment ID if not set
        if (job.Id == 0)
            job.Id = _jobs.Count > 0 ? _jobs.Max(j => j.Id) + 1 : 1;
        _jobs.Add(Clone(job));
        return Task.CompletedTask;
    }

    public Task<Job?> GetByGuidAsync(string guid)
    {
        if (guid == null) throw new ArgumentNullException(nameof(guid));
        var job = _jobs.FirstOrDefault(j => j.Guid == guid);
        return Task.FromResult(job != null ? Clone(job) : null);
    }

    public Task<Job?> GetByIdAsync(long id)
    {
        var job = _jobs.FirstOrDefault(j => j.Id == id);
        return Task.FromResult(job != null ? Clone(job) : null);
    }

    public Task<Job?> GetOneAsync(string tenantId, long? projectId, PipelineJobStatus status, string[] languages)
    {
        if (tenantId == null) throw new ArgumentNullException(nameof(tenantId));
        if (languages == null) throw new ArgumentNullException(nameof(languages));

        var job = _jobs.FirstOrDefault(j =>
            j.Status == status &&
            (projectId == null || j.TestProjectId == projectId) &&
            languages.Contains(j.Language)
        );
        return Task.FromResult(job != null ? Clone(job) : null);
    }

    public Task UpdateAsync(Job job)
    {
        if (job == null) throw new ArgumentNullException(nameof(job));
        var idx = _jobs.FindIndex(j => j.Id == job.Id);
        if (idx >= 0)
        {
            _jobs[idx] = Clone(job);
        }
        return Task.CompletedTask;
    }

    // Helper to avoid reference issues in tests
    private static Job Clone(Job job)
    {
        return new Job
        {
            Id = job.Id,
            Guid = job.Guid,
            Priority = job.Priority,
            Duration = job.Duration,
            Status = job.Status,
            Script = job.Script,
            Language = job.Language,
            EnvironmentVariables = new Dictionary<string, string>(job.EnvironmentVariables),
            ErrorMessage = job.ErrorMessage,
            StdOut = job.StdOut,
            StdErr = job.StdErr,
            Result = job.Result,
            Format = job.Format,
            ArtifactContent = job.ArtifactContent != null ? new Dictionary<string, byte[]>(job.ArtifactContent) : null,
            TestRunId = job.TestRunId,
            TestRun = job.TestRun,
            TeamId = job.TeamId,
            TestProjectId = job.TestProjectId,
            TestProject = job.TestProject,
            Team = job.Team,
            TenantId = job.TenantId,
            Tenant = job.Tenant
        };
    }
}