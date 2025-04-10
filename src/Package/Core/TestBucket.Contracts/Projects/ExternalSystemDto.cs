namespace TestBucket.Contracts.Projects;
public record class ExternalSystemDto
{
    /// <summary>
    /// Name of the external system
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Base URL in the external system for API access
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// ID for the project in the external system
    /// </summary>
    public string? ExternalProjectId { get; set; }

    /// <summary>
    /// Access token for the external system
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// API provides only read-only access
    /// </summary>
    public bool ReadOnly { get; set; }

    /// <summary>
    /// True if enabled
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Enabled capabilities. This can be configured by the user
    /// </summary>
    public ExternalSystemCapability EnabledCapabilities { get; set; }

    /// <summary>
    /// Supported capabilities. This is defined in the extension
    /// </summary>
    public ExternalSystemCapability SupportedCapabilities { get; set; }
}
