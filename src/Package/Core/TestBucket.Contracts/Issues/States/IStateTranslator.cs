namespace TestBucket.Contracts.Issues.States;

/// <summary>
/// Translates between internal and external states
/// </summary>
public interface IStateTranslator
{
    /// <summary>
    /// Returns the best internal state match from external state/status names. 
    /// </summary>
    /// <param name="externalStateName">The name of the state in the external system</param>
    /// <returns></returns>
    IssueState GetInternalIssueState(string externalStateName);

    /// <summary>
    /// Returns the best external state match from external state/status names. 
    /// </summary>
    /// <param name="externalStateNames">The possible external states to select from</param>
    /// <param name="mappedState">The current issue state</param>
    /// <param name="stateName">The current issue state name</param>
    /// <returns></returns>
    string GetExternalIssueState(MappedIssueState mappedState, string stateName, string[] externalStateNames);
}
