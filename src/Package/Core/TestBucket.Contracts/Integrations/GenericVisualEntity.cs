using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Integrations;
public record class GenericVisualEntity
{
    /// <summary>
    /// Title of item
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Color of item
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// SVG
    /// </summary>
    public string? Icon { get; set; }
}
