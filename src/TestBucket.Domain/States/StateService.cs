using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Projects;

namespace TestBucket.Domain.States;
public class StateService : IStateService
{
    private readonly IProjectRepository _projectRepository;

    public StateService(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    /// <summary>
    /// Returns default states if no states has been configured for the project
    /// </summary>
    /// <returns></returns>
    public TestState[] GetDefaultStates()
    {
        return [
                new() { Name = "Not Started", IsInitial = true },
                new() { Name = "Assigned" },
                new() { Name = "Ongoing" },
                new() { Name = "Completed", IsFinal = true },
            ];
    }

    public async Task<TestState> GetProjectInitialStateAsync(string tenantId, long projectId)
    {
        var states = await GetProjectStatesAsync(tenantId, projectId);
        var state = states.Where(x => x.IsInitial).FirstOrDefault();
        if (state is null)
        {
            states = GetDefaultStates();
            state = states.Where(x => x.IsInitial).First();
        }
        return state;
    }

    public async Task<TestState> GetProjectFinalStateAsync(string tenantId, long projectId)
    {
        var states = await GetProjectStatesAsync(tenantId, projectId);
        var state = states.Where(x => x.IsFinal).FirstOrDefault();
        if (state is null)
        {
            states = GetDefaultStates();
            state = states.Where(x => x.IsFinal).First();
        }
        return state;
    }


    public async Task<TestState[]> GetProjectStatesAsync(string tenantId, long projectId)
    {
        var project = await _projectRepository.GetProjectByIdAsync(tenantId, projectId);
        if(project?.TestStates is not null && project.TestStates.Length > 0)
        {
            return project.TestStates;
        }
        return GetDefaultStates();
    }
}
