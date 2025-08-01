using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Traits.Core;

namespace TestBucket.Traits.MSTest;

/// <summary>
/// Short-hand for CoveredIssueAttribute
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class BugAttribute : TestPropertyAttribute
{
    public BugAttribute(string issue) : base(TestTraitNames.CoveredIssue, issue)
    {
    }
}
