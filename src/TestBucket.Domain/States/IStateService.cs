
using System.Security.Claims;

namespace TestBucket.Domain.States;

public interface IStateService
{
    TestState[] GetDefaultStates();
    Task<TestState[]> GetProjectStatesAsync(ClaimsPrincipal principal, long projectId);

    /// <summary>
    /// Returns the final/completed state for a test
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    Task<TestState> GetProjectFinalStateAsync(ClaimsPrincipal principal, long projectId);

    /// <summary>
    /// Returns the initial state for a test
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    Task<TestState> GetProjectInitialStateAsync(ClaimsPrincipal principal, long projectId);
}