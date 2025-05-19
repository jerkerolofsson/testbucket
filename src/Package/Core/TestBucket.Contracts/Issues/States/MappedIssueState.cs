using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Issues.States;
public enum MappedIssueState
{
    Open = 1,
    Triage = 2,
    Triaged = 3,
    Accepted = 4,
    Assigned = 5,
    InProgress = 6,
    Reviewed = 7,
    Completed = 8,
    Delivered = 9,
    Closed = 10,
    Canceled = 11,



    /// <summary>
    /// Other, used defined with no mapping to internal state
    /// </summary>
    Other = 0,
}
