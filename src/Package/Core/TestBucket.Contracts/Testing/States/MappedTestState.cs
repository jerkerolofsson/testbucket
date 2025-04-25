using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Testing.States;

/// <summary>
/// States for test execution
/// </summary>
public enum MappedTestState
{
    NotStarted,

    Assigned,

    Ongoing,

    Completed,

    /// <summary>
    /// Other, used defined with no mapping to internal state
    /// </summary>
    Other,
}
