using TestBucket.Domain.States.Models;

namespace TestBucket.Domain.States;
public interface IStateRepository
{
    /// <summary>
    /// Returns state definitions for the given project for all entity types
    /// This does not include team or tenant definitions
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    Task<StateDefinition?> GetProjectStateDefinitionAsync(string tenantId, long projectId);

    /// <summary>
    /// Returns the state definitions for the given team for all entity types
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="teamId"></param>
    /// <returns></returns>
    Task<StateDefinition?> GetTeamStateDefinitionAsync(string tenantId, long teamId);

    /// <summary>
    /// Returns the state definitions for the given tenant for all entity types
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="teamId"></param>
    /// <returns></returns>
    Task<StateDefinition?> GetTenantStateDefinitionAsync(string tenantId);

    Task AddStateDefinitionAsync(StateDefinition stateDefinition);
    Task UpdateStateDefinitionAsync(StateDefinition stateDefinition);
    Task DeleteStateDefinitionAsync(StateDefinition stateDefinition);
}
