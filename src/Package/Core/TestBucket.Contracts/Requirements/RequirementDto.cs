using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Requirements.States;
using TestBucket.Contracts.Requirements.Types;
using TestBucket.Formats.Dtos;

namespace TestBucket.Contracts.Requirements;
public class RequirementDto : RequirementEntityDto
{
    public string? State { get; set; }
    public string? Path { get; set; }
    public string? Slug { get; set; }
    public MappedRequirementState? MappedState { get; set; }
    public string? RequirementType { get; set; }
    public MappedRequirementType? MappedType { get; set; }

    /// <summary>
    /// Slug for the project
    /// </summary>
    public string? ProjectSlug { get; set; }

    /// <summary>
    /// Slug for the parent directory
    /// </summary>
    public string? ParentRequirementSlug { get; set; }

    /// <summary>
    /// Contains traits/custom properties
    /// </summary>
    public TestTraitCollection? Traits { get; set; }
}
