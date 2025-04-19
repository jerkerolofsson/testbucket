
using TestBucket.Domain.Automation.Runners.Models;

namespace TestBucket.Domain.Automation.Runners;
public interface IRunnerManager
{
    /// <summary>
    /// Returns all runners
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    Task<IReadOnlyList<Runner>> GetAllAsync(ClaimsPrincipal principal);

    /// <summary>
    /// Deletes a runner
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="runner"></param>
    /// <returns></returns>
    Task RemoveAsync(ClaimsPrincipal principal, Runner runner);
}