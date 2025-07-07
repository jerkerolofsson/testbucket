using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Issues.States;

namespace TestBucket.Jira.Models;
internal class DefaultStateMap : IssueStateMapping
{
    public DefaultStateMap()
    {
        // Initialize the mapping with default values
        AddMapping("To Do", new IssueState(IssueStates.Open, MappedIssueState.Open));
        AddMapping("In Progress", new IssueState(IssueStates.InProgress, MappedIssueState.InProgress));
        AddMapping("Done", new IssueState(IssueStates.Closed, MappedIssueState.Closed));
        AddMapping("In Review", new IssueState(IssueStates.InReview, MappedIssueState.InReview));
    }
}
