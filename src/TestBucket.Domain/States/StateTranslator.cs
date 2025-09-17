using TestBucket.Contracts.Issues.States;
using TestBucket.Domain.States.Models;

namespace TestBucket.Domain.States;

internal class StateTranslator : IStateTranslator
{
    private readonly IReadOnlyList<IssueState> _issueStates;

    public StateTranslator(IReadOnlyList<IssueState> issueStates)
    {
        _issueStates = issueStates;
    }

    public string GetExternalIssueState(MappedIssueState mappedState, string stateName, string[] externalStateNames)
    {
        // 1. Try to get a match from MappedState matching exact name
        foreach (var state in _issueStates.Where(x => x.MappedState == mappedState))
        {
            foreach (var externalStateName in externalStateNames)
            {
                if (state.Name is not null && state.Name.Equals(externalStateName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return externalStateName;
                }
            }
        }

        // 2. Try to get a match from state name matching an alias
        foreach (var state in _issueStates.Where(x => x.MappedState == mappedState))
        {
            foreach (var externalStateName in externalStateNames)
            {
                if (state.Aliases is not null)
                {
                    foreach (var alias in state.Aliases)
                    {
                        if (alias is not null && alias.Equals(externalStateName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            return externalStateName;
                        }
                    }
                }
            }
        }

        // 3. Ingore mapped state, just compare by names and aliases
        foreach (var state in _issueStates)
        {
            foreach (var externalStateName in externalStateNames)
            {
                if (state.Name is not null && state.Name.Equals(externalStateName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return externalStateName;
                }
                if (state.Aliases is not null)
                {
                    foreach (var alias in state.Aliases)
                    {
                        if (alias is not null && alias.Equals(externalStateName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            return externalStateName;
                        }
                    }
                }
            }
        }

        return externalStateNames.First();
    }

    public IssueState GetInternalIssueState(string externalStateName)
    {
        // 1. Try to get by exact name match
        foreach (var state in _issueStates)
        {
            if (state.Name is not null && state.Name.Equals(externalStateName, StringComparison.InvariantCultureIgnoreCase))
            {
                return state;
            }
        }

        // 2. Get by aliases
        foreach (var state in _issueStates)
        {
            if (state.Aliases is not null)
            {
                foreach(var stateName in state.Aliases)
                {
                    if (stateName is not null && stateName.Equals(externalStateName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return state;
                    }
                }
            }
        }

        return new IssueState("Other", MappedIssueState.Other);
    }
}
