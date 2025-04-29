using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Code.Models;

namespace TestBucket.Domain.Code.Services;
public interface ICommitManager
{
    Task AddCommitAsync(ClaimsPrincipal principal, Commit commit);
    Task AddRepositoryAsync(ClaimsPrincipal principal, Repository repo);
    Task<Repository?> GetRepoByExternalSystemAsync(ClaimsPrincipal principal, long id);
    Task UpdateRepositoryAsync(ClaimsPrincipal principal, Repository repo);

    Task<PagedResult<Commit>> BrowseCommitsAsync(ClaimsPrincipal principal, long projectId, int offset, int count);
    Task UpdateCommitAsync(ClaimsPrincipal principal, Commit commit);
}
