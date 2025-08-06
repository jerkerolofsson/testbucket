using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.States;
using TestBucket.Domain.States.Models;

namespace TestBucket.Domain.UnitTests.States.Fakes;
internal class FakeStateRepository : IStateRepository
{
    private readonly List<StateDefinition> _stateDefinitions = new();
    private long _idCounter = 1;

    public Task AddStateDefinitionAsync(StateDefinition stateDefinition)
    {
        stateDefinition.Id = _idCounter;
        _idCounter++;
        _stateDefinitions.Add(stateDefinition);
        return Task.CompletedTask;
    }

    public Task DeleteStateDefinitionAsync(StateDefinition stateDefinition)
    {
        _stateDefinitions.Remove(stateDefinition);
        return Task.CompletedTask;
    }

    public Task<StateDefinition?> GetProjectStateDefinitionAsync(string tenantId, long projectId)
    {
        var stateDefinition = _stateDefinitions.FirstOrDefault(sd => sd.TenantId == tenantId && sd.TestProjectId == projectId);
        return Task.FromResult(stateDefinition);
    }

    public Task<StateDefinition?> GetTeamStateDefinitionAsync(string tenantId, long teamId)
    {
        var stateDefinition = _stateDefinitions.FirstOrDefault(sd => sd.TenantId == tenantId && sd.TeamId == teamId && sd.TestProjectId == null);
        return Task.FromResult(stateDefinition);
    }
  
    public Task<StateDefinition?> GetTenantStateDefinitionAsync(string tenantId)
    {
        var stateDefinition = _stateDefinitions.FirstOrDefault(sd => sd.TenantId == tenantId && sd.TeamId == null && sd.TestProjectId == null);
        return Task.FromResult(stateDefinition);
    }

    public Task UpdateStateDefinitionAsync(StateDefinition stateDefinition)
    {
        var existing = _stateDefinitions.FirstOrDefault(sd => sd.Id == stateDefinition.Id);
        if (existing != null)
        {
            _stateDefinitions.Remove(existing);
            _stateDefinitions.Add(stateDefinition);
            return Task.CompletedTask;
        }
        throw new Exception($"No state definition with id: {stateDefinition.Id} was found!");
    }
}
