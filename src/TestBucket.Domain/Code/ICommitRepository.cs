using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Code;

/// <summary>
/// Persistance storage for commits.
/// Commits are indexed
/// </summary>
public interface ICommitRepository
{
    #region Repo
    Task<Repository?> GetRepoByUrlAsync(string tenantId, string url);
    Task<Repository?> GetRepoByExternalSystemAsync(long externalSystemId);
    Task AddRepositoryAsync(Repository repo);
    Task UpdateRepositoryAsync(Repository repo);
    #endregion Repo

    #region Commit
    /// <summary>
    /// Returns a commit by SHA
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="sha"></param>
    /// <returns></returns>
    Task<Commit?> GetCommitByShaAsync(string tenantId, string sha);

    /// <summary>
    /// Searches for commits
    /// </summary>
    /// <param name="filters"></param>
    /// <returns></returns>
    Task<PagedResult<Commit>> SearchCommitsAsync(FilterSpecification<Commit>[] filters, int offset, int count);
    Task AddCommitAsync(Commit value);
    #endregion Commit
}
