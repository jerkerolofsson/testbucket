using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Requirements.States;

namespace TestBucket.Contracts.Requirements.Types;
public class RequirementType
{
    /// <summary>
    /// Name of the state (this can be anything, defined by external systems)
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Mapped internal type
    /// </summary>
    public MappedRequirementType MappedType { get; set; }

}
