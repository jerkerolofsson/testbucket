using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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

    public async Task<TestState> GetProjectInitialStateAsync(ClaimsPrincipal principal, long projectId)
    {
        var states = await GetProjectStatesAsync(principal, projectId);
        var state = states.Where(x => x.IsInitial).FirstOrDefault();
        if (state is null)
        {
            states = GetDefaultStates();
            state = states.Where(x => x.IsInitial).First();
        }
        return state ?? new TestState() { Name = "Not Started", IsFinal = false, IsInitial = true };
    }

    public async Task<TestState> GetProjectFinalStateAsync(ClaimsPrincipal principal, long projectId)
    {
        var states = await GetProjectStatesAsync(principal, projectId);
        var state = states.Where(x => x.IsFinal).FirstOrDefault();
        if (state is null)
        {
            states = GetDefaultStates();
            state = states.Where(x => x.IsFinal).First();
        }
        return state ?? new TestState() { Name = "Completed", IsFinal = true, IsInitial = false }; ;
    }


    public async Task<TestState[]> GetProjectStatesAsync(ClaimsPrincipal principal, long projectId)
    {
        var tenantId = principal.GetTentantIdOrThrow();
        var project = await _projectRepository.GetProjectByIdAsync(tenantId, projectId);
        if(project?.TestStates is not null && project.TestStates.Length > 0)
        {
            return project.TestStates;
        }
        return GetDefaultStates();
    }
}
