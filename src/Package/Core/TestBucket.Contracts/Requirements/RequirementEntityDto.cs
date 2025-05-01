using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Requirements;

/// <summary>
/// Base class for folders or requirements
/// </summary>
public class RequirementEntityDto
{
    public string? Slug { get; set; }

    /// <summary>
    /// Slug for the project
    /// </summary>
    public string? ProjectSlug { get; set; }

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
}
