using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Requirements.States;

/// <summary>
/// Known requirement state. 
/// </summary>
public enum MappedRequirementState
{
    Draft,
    Accepted,
    Assigned,
    InProgress,
    Reviewed,
    Completed,
    Delivered,
    Canceled,

    /// <summary>
    /// Other, used defined with no mapping to internal state
    /// </summary>
    Other,
}
