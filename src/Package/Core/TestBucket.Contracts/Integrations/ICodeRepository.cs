using TestBucket.Contracts.Code.Models;

namespace TestBucket.Contracts.Integrations;
public interface ICodeRepository
{
    string SystemName { get; }

    /// <summary>
    /// Returns a specific commit
    /// </summary>
    /// <param name="system"></param>
    /// <param name="reference"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CommitDto> GetCommitAsync(ExternalSystemDto system, string reference, CancellationToken cancellationToken);

    /// <summary>
    /// Searches for commits
    /// </summary>
    /// <param name="system"></param>
    /// <param name="sha"></param>
    /// <param name="since"></param>
    /// <param name="until"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyList<CommitDto>> GetCommitsAsync(ExternalSystemDto system, string? sha, DateTimeOffset? since, DateTimeOffset? until, CancellationToken cancellationToken);
}