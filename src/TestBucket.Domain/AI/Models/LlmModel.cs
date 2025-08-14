using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.AI.Models;
/// <summary>
/// Represents a Large Language Model (LLM) configuration, including its display icon, name, vendor, and supported capabilities.
/// </summary>
public class LlmModel
{
    /// <summary>
    /// Price for 1 000 000 tokens in USD.
    /// </summary>
    public double UsdPerMillionInputTokens { get; set; }

    /// <summary>
    /// Price for 1 000 000 tokens in USD.
    /// </summary>
    public double UsdPerMillionOutputTokens { get; set; }

    /// <summary>
    /// Gets or sets the SVG icon representing the model.
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Gets or sets the display name of the model.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the vendor or provider of the model (e.g., meta, microsoft).
    /// </summary>
    public string? Vendor { get; set; }

    /// <summary>
    /// Gets or sets the model name as used by the API
    /// </summary>
    public string? ModelName { get; set; }

    /// <summary>
    /// Gets or sets the model name as used by Azure, if applicable.
    /// </summary>
    public string? AzureName { get; set; }

    /// <summary>
    /// Gets or sets the capabilities supported by the model.
    /// </summary>
    public ModelCapability Capabilities { get; set; }
}