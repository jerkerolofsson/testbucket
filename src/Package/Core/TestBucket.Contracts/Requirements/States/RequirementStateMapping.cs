using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Issues.States;

namespace TestBucket.Contracts.Requirements.States;

/// <summary>
/// A rule for issue state mapping
/// </summary>
public class RequirementStateMapping : Dictionary<string, IssueState>
{
}
