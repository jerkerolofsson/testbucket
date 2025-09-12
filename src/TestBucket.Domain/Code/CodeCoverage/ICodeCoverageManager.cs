using TestBucket.Domain.Code.CodeCoverage.Models;

namespace TestBucket.Domain.Code.CodeCoverage;
public interface ICodeCoverageManager
{
    /// <summary>
    /// Gets an existing or returns a code coverage group based on the type and name
    /// </summary>
    /// <param name="user"></param>
    /// <param name="groupType"></param>
    /// <param name="groupName"></param>
    /// <returns></returns>
    Task<CodeCoverageGroup> GetOrCreateCodeCoverageGroupAsync(ClaimsPrincipal user, long projectId, CodeCoverageGroupType groupType, string groupName);

    /// <summary>
    /// Loads code coverage settings
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    Task<CodeCoverageSettings> LoadSettingsAsync(ClaimsPrincipal principal, long projectId);

    /// <summary>
    /// Updates a code coverage group
    /// </summary>
    /// <param name="user"></param>
    /// <param name="group"></param>
    /// <returns></returns>
    Task UpdateCodeCoverageGroupAsync(ClaimsPrincipal user, CodeCoverageGroup group);
}