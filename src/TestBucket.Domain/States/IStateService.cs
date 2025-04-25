
using System.Collections.Generic;
using System.Security.Claims;

using TestBucket.Contracts.Requirements.States;
using TestBucket.Contracts.Testing.States;

namespace TestBucket.Domain.States;

public interface IStateService
{
    /// <summary>
    /// Returns list of all states that can be set for a requirement
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    Task<IReadOnlyList<RequirementState>> GetRequirementStatesAsync(ClaimsPrincipal principal, long projectId);


    /// <summary>
    /// Returns list of all states that can be set for a test case
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    Task<IReadOnlyList<TestState>> GetTestCaseRunStatesAsync(ClaimsPrincipal principal, long projectId);

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