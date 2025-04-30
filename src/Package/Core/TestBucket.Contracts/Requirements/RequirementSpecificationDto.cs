using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Requirements;

/// <summary>
/// Root level for a collection of requirements
/// </summary>
public class RequirementSpecificationDto
{
    /// <summary>
    /// Is the specification read-only?
    /// </summary>
    public bool ReadOnly { get; set; }

    /// <summary>
    /// Name of the specification
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Identifier in external systme
    /// </summary>
    public string? ExternalId { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Name of external provider
    /// </summary>
    public string? Provider { get; set; }

    /// <summary>
    /// Icon (SVG)
    /// </summary>
    public string? Icon { get; set; }
    public string? Color { get; set; }

    /// <summary>
    /// Project slug
    /// </summary>
    public string? ProjectSlug { get; set; }
    public string? Slug { get; set; }
}
