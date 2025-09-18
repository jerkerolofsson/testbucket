using Mediator;

using TestBucket.Domain.Code.Events;
using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Code.Specifications;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Code.Services;
public class CommitManager : ICommitManager
{
    private readonly ICommitRepository _repo;
    private readonly IMediator _mediator;
    private readonly TimeProvider _timeProvider;

    public CommitManager(
        ICommitRepository repo, 
        IMediator mediator,
        TimeProvider timeProvider)
    {
        _repo = repo;
        _mediator = mediator;
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// Searches for commits and returns the result
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="projectId"></param>
    /// <param name="text"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public async Task<PagedResult<Commit>> SearchCommitsAsync(ClaimsPrincipal principal, long projectId, string text, int offset, int count)
    {
        ArgumentNullException.ThrowIfNull(principal);
        principal.ThrowIfNoPermission(PermissionEntityType.Architecture, PermissionLevel.Read);
        FilterSpecification<Commit>[] filters = [
            new FilterByTenant<Commit>(principal.GetTenantIdOrThrow()),
            new FilterByProject<Commit>(projectId),
            new SearchCommitWithText(text)
            ];

        var result = await _repo.SearchCommitsAsync(filters, offset, count);
        return result;
    }

    public Task<PagedResult<Commit>> BrowseCommitsAsync(ClaimsPrincipal principal, long projectId, int offset, int count)
    {
        ArgumentNullException.ThrowIfNull(principal);
        principal.ThrowIfNoPermission(PermissionEntityType.Architecture, PermissionLevel.Read);
        FilterSpecification<Commit>[] filters = [
            new FilterByTenant<Commit>(principal.GetTenantIdOrThrow()),
            new FilterByProject<Commit>(projectId)
            ];

        return _repo.SearchCommitsAsync(filters, offset, count);
    }

    public async Task<Commit?> GetCommitByShaAsync(ClaimsPrincipal principal, string sha)
    {
        ArgumentNullException.ThrowIfNull(principal);
        principal.ThrowIfNoPermission(PermissionEntityType.Architecture, PermissionLevel.Read);

        return await _repo.GetCommitByShaAsync(principal.GetTenantIdOrThrow(), sha);
    }


    public async Task AddCommitAsync(ClaimsPrincipal principal, Commit commit)
    {
        ArgumentNullException.ThrowIfNull(commit);
        ArgumentNullException.ThrowIfNull(principal);
        ArgumentNullException.ThrowIfNull(commit.TeamId);
        ArgumentNullException.ThrowIfNull(commit.TestProjectId);
        principal.ThrowIfNoPermission(PermissionEntityType.Architecture, PermissionLevel.Write);

        var projectId = commit.TestProjectId.Value;

        commit.TenantId = principal.GetTenantIdOrThrow();
        commit.Modified = commit.Created = _timeProvider.GetUtcNow();
        commit.CreatedBy = principal.Identity?.Name;
        commit.ModifiedBy = principal.Identity?.Name;

        await _repo.DeleteCommitByShaAsync(commit.Sha);
        await _repo.AddCommitAsync(commit);

        await _mediator.Publish(new CommitAddedEvent(principal, commit));
    }

    public async Task AddRepositoryAsync(ClaimsPrincipal principal, Repository repo)
    {
        ArgumentNullException.ThrowIfNull(principal);
        ArgumentNullException.ThrowIfNull(repo.TeamId);
        ArgumentNullException.ThrowIfNull(repo.TestProjectId);
        principal.ThrowIfNoPermission(PermissionEntityType.Architecture, PermissionLevel.Write);

        repo.TenantId = principal.GetTenantIdOrThrow();
        repo.Modified = repo.Created = _timeProvider.GetUtcNow();
        repo.CreatedBy = principal.Identity?.Name;
        repo.ModifiedBy = principal.Identity?.Name;
        await _repo.AddRepositoryAsync(repo);
    }

    public async Task<Repository?> GetRepoByExternalSystemAsync(ClaimsPrincipal principal, long id)
    {
        ArgumentNullException.ThrowIfNull(principal);
        principal.ThrowIfNoPermission(PermissionEntityType.Architecture, PermissionLevel.Read);
        return await _repo.GetRepoByExternalSystemAsync(id);
    }

    public async Task UpdateRepositoryAsync(ClaimsPrincipal principal, Repository repo)
    {
        ArgumentNullException.ThrowIfNull(principal);
        ArgumentNullException.ThrowIfNull(repo);
        principal.ThrowIfNoPermission(PermissionEntityType.Architecture, PermissionLevel.Write);

        repo.Modified = _timeProvider.GetUtcNow();
        repo.ModifiedBy = principal.Identity?.Name;
        await _repo.UpdateRepositoryAsync(repo);
    }

    public async Task UpdateCommitAsync(ClaimsPrincipal principal, Commit commit)
    {
        ArgumentNullException.ThrowIfNull(principal);
        ArgumentNullException.ThrowIfNull(commit);
        principal.ThrowIfNoPermission(PermissionEntityType.Architecture, PermissionLevel.Write);

        commit.Modified = _timeProvider.GetUtcNow();
        commit.ModifiedBy = principal.Identity?.Name;
        await _repo.UpdateCommitAsync(commit);
    }
}
