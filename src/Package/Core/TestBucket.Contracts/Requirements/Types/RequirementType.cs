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

    public override string ToString() => Name??"";

    public override bool Equals(object? obj)
    {
        if (obj is RequirementType state)
        {
            return state.MappedType == MappedType ||
                (state.MappedType == MappedRequirementType.Other && state.Name == Name);
        }
        return false;
    }

    public override int GetHashCode() => MappedType.GetHashCode();

}
