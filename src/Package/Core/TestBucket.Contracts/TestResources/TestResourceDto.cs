using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace TestBucket.Contracts.TestResources;
public class TestResourceDto
{
    /// <summary>
    /// Owner of the resource (resource server identifier)
    /// </summary>
    public required string Owner { get; set; }

    /// <summary>
    /// Locally unique resource ID
    /// </summary>
    public required string ResourceId { get; set; }

    /// <summary>
    /// Device model
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// Device manufacturer
    /// </summary>
    public string? Manufacturer { get; set; }

    /// <summary>
    /// Device health
    /// </summary>
    public HealthStatus Health { get; set; }

    /// <summary>
    /// Device types
    /// </summary>
    public required string[] Types { get; set; }

    /// <summary>
    /// Variables
    /// </summary>
    public Dictionary<string, string> Variables { get; set; } = [];

    /// <summary>
    /// Resource name
    /// </summary>
    public required string Name { get; set; }
}
