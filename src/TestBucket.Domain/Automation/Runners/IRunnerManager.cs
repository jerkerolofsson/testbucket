
using TestBucket.Domain.Automation.Runners.Models;

namespace TestBucket.Domain.Automation.Runners;
public interface IRunnerManager
{
    /// <summary>
    /// Adds a runner
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="runner"></param>
    /// <returns></returns>
    Task AddAsync(ClaimsPrincipal principal, Runner runner);

    /// <summary>
    /// Returns all runners
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    Task<IReadOnlyList<Runner>> GetAllAsync(ClaimsPrincipal principal);

    /// <summary>
    /// Returns a runner by id, or null if not found
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<Runner?> GetByIdAsync(ClaimsPrincipal principal, string id);

    /// <summary>
    /// Deletes a runner
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="runner"></param>
    /// <returns></returns>
    Task RemoveAsync(ClaimsPrincipal principal, Runner runner);

    /// <summary>
    /// Updates a runner
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="runner"></param>
    /// <returns></returns>
    Task UpdateAsync(ClaimsPrincipal principal, Runner runner);
}