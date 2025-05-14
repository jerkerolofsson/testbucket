using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Issues.Types;
public enum MappedIssueType
{
    /// <summary>
    /// Issue
    /// </summary>
    Issue,

    /// <summary>
    /// Enhancement
    /// </summary>
    Enhancement,

    /// <summary>
    /// Incident
    /// </summary>
    Incident,

    /// <summary>
    /// Question
    /// </summary>
    Question,

    /// <summary>
    /// Other, used defined with no mapping to internal state
    /// </summary>
    Other,
}
