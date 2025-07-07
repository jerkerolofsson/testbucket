using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Issues.States;

/// <summary>
/// A rule for issue state mapping
/// </summary>
public class IssueStateMapping : Dictionary<string, IssueState>
{
    public void AddMapping(string name, IssueState issueState)
    {
        this[name] = issueState;
    }

}
