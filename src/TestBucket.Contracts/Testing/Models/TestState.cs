using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Testing.Models;
public class TestState
{
    /// <summary>
    /// Name of the state
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Flag that indicates that this state is the initial state (e.g. Not Started)
    /// </summary>
    public bool IsInitial { get; set; }

    /// <summary>
    /// Flag that indicates that this state is the final state (e.g. Completed)
    /// </summary>
    public bool IsFinal { get; set; }

}
