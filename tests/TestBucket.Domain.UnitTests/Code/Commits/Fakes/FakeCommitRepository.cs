using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestBucket.Domain.Code;
using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Contracts;

namespace TestBucket.Domain.UnitTests.Code.Commits.Fakes;

internal class FakeCommitRepository : ICommitRepository
{
    private readonly List<Commit> _commits = new();
    private readonly List<Repository> _repositories = new();
    private long _idCounter;

    public Task AddCommitAsync(Commit value)
    {
        value.Id = ++_idCounter;
        _commits.RemoveAll(c => c.Sha == value.Sha);
        _commits.Add(value);
        return Task.CompletedTask;
    }

    public Task DeleteCommitByShaAsync(string sha)
    {
        _commits.RemoveAll(c => c.Sha == sha);
        return Task.CompletedTask;
    }

    public Task DeleteCommitAsync(long id)
    {
        _commits.RemoveAll(c => c.Id == id);
        return Task.CompletedTask;
    }

    public Task UpdateCommitAsync(Commit commit)
    {
        var idx = _commits.FindIndex(c => c.Id == commit.Id);
        if (idx >= 0)
            _commits[idx] = commit;
        return Task.CompletedTask;
    }

    public Task<Commit?> GetCommitByShaAsync(string tenantId, string sha)
    {
        var commit = _commits.FirstOrDefault(c => c.TenantId == tenantId && c.Sha == sha);
        return Task.FromResult(commit);
    }

    public Task<PagedResult<Commit>> SearchCommitsAsync(FilterSpecification<Commit>[] filters, int offset, int count)
    {
        IEnumerable<Commit> query = _commits;
        foreach (var filter in filters)
        {
            query = query.Where(c => filter.IsMatch(c));
        }
        var items = query.Skip(offset).Take(count).ToArray();
        var result = new PagedResult<Commit>
        {
            TotalCount = query.LongCount(),
            Items = items
        };
        return Task.FromResult(result);
    }

    public Task AddRepositoryAsync(Repository repo)
    {
        repo.Id = ++_idCounter;
        _repositories.RemoveAll(r => r.Id == repo.Id || r.Url == repo.Url);
        _repositories.Add(repo);
        return Task.CompletedTask;
    }

    public Task UpdateRepositoryAsync(Repository repo)
    {
        var idx = _repositories.FindIndex(r => r.Id == repo.Id);
        if (idx >= 0)
            _repositories[idx] = repo;
        return Task.CompletedTask;
    }

    public Task<Repository?> GetRepoByUrlAsync(string tenantId, string url)
    {
        var repo = _repositories.FirstOrDefault(r => r.Url == url && r.TenantId == tenantId);
        return Task.FromResult(repo);
    }

    public Task<Repository?> GetRepoByExternalSystemAsync(long externalSystemId)
    {
        var repo = _repositories.FirstOrDefault(r => r.ExternalSystemId == externalSystemId);
        return Task.FromResult(repo);
    }
}