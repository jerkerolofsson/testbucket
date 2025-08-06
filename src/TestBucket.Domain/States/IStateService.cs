using TestBucket.Contracts.Issues.States;
using TestBucket.Contracts.Issues.Types;
using TestBucket.Contracts.Requirements.States;
using TestBucket.Contracts.Requirements.Types;
using TestBucket.Contracts.Testing.States;
using TestBucket.Domain.States.Models;

namespace TestBucket.Domain.States;

public interface IStateService
{
    /// <summary>
    /// Returns list of all states that can be set for am issue
    /// The types are merged and contains both team and project definitions
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    Task<IReadOnlyList<IssueState>> GetIssueStatesAsync(ClaimsPrincipal principal, long projectId);

    /// <summary>
    /// Returns list of all states that can be set for a requirement
    /// The types are merged and contains both team and project definitions
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    Task<IReadOnlyList<RequirementState>> GetRequirementStatesAsync(ClaimsPrincipal principal, long projectId);


    /// <summary>
    /// Returns list of all states that can be set for a test case
    /// The types are merged and contains both team and project definitions
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    Task<IReadOnlyList<TestState>> GetTestCaseRunStatesAsync(ClaimsPrincipal principal, long projectId);

    /// <summary>
    /// Returns the final/completed state for a test case run
    /// The types are merged and contains both team and project definitions
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    Task<TestState> GetTestCaseRunFinalStateAsync(ClaimsPrincipal principal, long projectId);

    /// <summary>
    /// Returns the initial state for a test case
    /// The types are merged and contains both team and project definitions
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    Task<TestState> GetTestCaseInitialStateAsync(ClaimsPrincipal principal, long projectId);

    /// <summary>
    /// Returns the initial state for a test case run
    /// The types are merged and contains both team and project definitions
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    Task<TestState> GetTestCaseRunInitialStateAsync(ClaimsPrincipal principal, long projectId);

    /// <summary>
    /// Returns requirement types for the project
    /// The types are merged and contains both team and project definitions
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    Task<IReadOnlyList<RequirementType>> GetRequirementTypesAsync(ClaimsPrincipal principal, long projectId);

    /// <summary>
    /// Returns issue types for the project
    /// The types are merged and contains both team and project definitions
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    Task<IReadOnlyList<IssueType>> GetIssueTypesAsync(ClaimsPrincipal principal, long projectId);

    /// <summary>
    /// Returns the team state definitions. This will read from the database
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="teamId"></param>
    /// <returns></returns>
    Task<StateDefinition> GetTeamDefinitionAsync(ClaimsPrincipal principal, long teamId);

    /// <summary>
    /// Returns project definitions. This will read from the database
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    Task<StateDefinition> GetProjectDefinitionAsync(ClaimsPrincipal principal, long projectId);

    /// <summary>
    /// Saves changes to either a team definition or project definition.
    /// This will invalidate the cache
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="stateDefinition"></param>
    /// <returns></returns>
    Task SaveDefinitionAsync(ClaimsPrincipal principal, StateDefinition stateDefinition);

    /// <summary>
    /// Deletes the definition
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="stateDefinition"></param>
    /// <returns></returns>
    Task DeleteDefinitionAsync(ClaimsPrincipal principal, StateDefinition stateDefinition);
    Task<StateDefinition> GetTenantDefinitionAsync(ClaimsPrincipal principal);
    Task<IReadOnlyList<TestState>> GetTestCaseStatesAsync(ClaimsPrincipal principal, long projectId);
}