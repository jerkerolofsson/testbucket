using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Issues.States;
public enum MappedIssueState
{
    Open,
    Triaged,
    Accepted,
    Assigned,
    InProgress,
    Reviewed,
    Completed,
    Delivered,
    Closed,
    Canceled,


    /// <summary>
    /// Other, used defined with no mapping to internal state
    /// </summary>
    Other,
}
