
namespace TestBucket.Domain.States;

public interface IStateService
{
    TestState[] GetDefaultStates();
    Task<TestState[]> GetProjectStatesAsync(string tenantId, long projectId);

    /// <summary>
    /// Returns the final/completed state for a test
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    Task<TestState> GetProjectFinalStateAsync(string tenantId, long projectId);

    /// <summary>
    /// Returns the initial state for a test
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    Task<TestState> GetProjectInitialStateAsync(string tenantId, long projectId);
}