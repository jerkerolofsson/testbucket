using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Requirements.Models;

namespace TestBucket.Domain.Code.Services;
public class CommitManager : ICommitManager
{
    private readonly ICommitRepository _repo;

    public CommitManager(ICommitRepository repo)
    {
        _repo = repo;
    }

    public async Task AddCommitAsync(ClaimsPrincipal principal, Commit commit)
    {
        commit.Created = DateTimeOffset.UtcNow;
        commit.Modified = DateTimeOffset.UtcNow;
        commit.CreatedBy = principal.Identity?.Name;
        commit.ModifiedBy = principal.Identity?.Name;

        // Scan for impacted feature/component/layer

        if (commit.CommitFiles is not null)
        {
            var files = commit.CommitFiles.ToList();
            foreach(var file in files)
            {
                // Lookup impacted feature/component/layer/system
            }
            commit.CommitFiles = null;
            await _repo.AddCommitAsync(commit);
        }
        else
        {
            await _repo.AddCommitAsync(commit);
        }
    }

    public async Task AddRepositoryAsync(ClaimsPrincipal principal, Repository repo)
    {
        repo.Created = DateTimeOffset.UtcNow;
        repo.Modified = DateTimeOffset.UtcNow;
        repo.CreatedBy = principal.Identity?.Name;
        repo.ModifiedBy = principal.Identity?.Name;
        await _repo.AddRepositoryAsync(repo);
    }

    public async Task<Repository?> GetRepoByExternalSystemAsync(ClaimsPrincipal principal, long id)
    {
        return await _repo.GetRepoByExternalSystemAsync(id);
    }

    public async Task UpdateRepositoryAsync(ClaimsPrincipal principal, Repository repo)
    {
        repo.Modified = DateTimeOffset.UtcNow;
        repo.ModifiedBy = principal.Identity?.Name;
        await _repo.UpdateRepositoryAsync(repo);
    }
}
