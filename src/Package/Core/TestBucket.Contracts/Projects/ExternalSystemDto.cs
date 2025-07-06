namespace TestBucket.Contracts.Integrations;
public record class ExternalSystemDto
{
    /// <summary>
    /// Name of the external system
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// ID of the configurtion
    /// </summary>
    public required long Id { get; set; }

    /// <summary>
    /// Base URL in the external system for API access
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Unique identifier for the provider. This identifies the extension implementing it. 
    /// </summary>
    public string? Provider { get; set; }

    /// <summary>
    /// ID for the project in the external system
    /// </summary>
    public string? ExternalProjectId { get; set; }

    /// <summary>
    /// Access token for the external system
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// API Key for the external system
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Client ID (oauth)
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// Client Secret (oauth)
    /// </summary>
    public string? ClientSecret { get; set; }

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

    /// <summary>
    /// Glob pattern for test-result artifacts. Separated by ;
    /// </summary>
    public string? TestResultsArtifactsPattern { get; set; }

    /// <summary>
    /// Glob pattern for test-coverage report artifacts. Separated by ; 
    /// </summary>
    public string? CoverageReportArtifactsPattern { get; set; }
}
