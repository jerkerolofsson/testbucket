using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace TestBucket.Contracts.TestResources;
public class TestResourceDto
{
    public required string Owner { get; set; }

    /// <summary>
    /// Locally unique resource ID
    /// </summary>
    public required string ResourceId { get; set; }
    public string? Model { get; set; }
    public string? Manufacturer { get; set; }
    public HealthStatus Health { get; set; }

    /// <summary>
    /// Device types
    /// </summary>
    public required string[] Types { get; set; }

    /// <summary>
    /// Variables
    /// </summary>
    public Dictionary<string, string> Variables { get; set; } = [];
    public required string Name { get; set; }
}
