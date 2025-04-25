using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Requirements.Types;
public enum MappedRequirementType
{
    /// <summary>
    /// General requirement
    /// </summary>
    General,

    /// <summary>
    /// A regulatory requirement
    /// </summary>
    Regulatory,

    /// <summary>
    /// Part of a standard's specification
    /// </summary>
    Standard,

    Epic,
    Story,
    Initiative,
    Task,

    /// <summary>
    /// Other, used defined with no mapping to internal state
    /// </summary>
    Other,
}
