using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Traceability.Models;
public class TraceabilityNode
{
    public Requirement? Requirement { get; init; }
    public TestCase? TestCase { get; init; }

    public string Name => Requirement.Name ?? TestCase.Name ?? "";

    /// <summary>
    /// Upstream dependencies
    /// 
    /// This could be parent items or dependencies
    /// </summary>
    public List<TraceabilityNode> Upstream { get; } = [];

    /// <summary>
    /// Downstream dependencies
    /// 
    /// This could be child requirements, or test cases
    /// </summary>
    public List<TraceabilityNode> Downstream { get; } = [];
}
