namespace TestBucket.Runner.Settings;

public class LocalRunnerSettings
{
    /// <summary>
    /// Access token
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// Name of runner
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// ID of runner
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Public base url for this service if it is behind a reverse proxy or similar
    /// </summary>
    public string? PublicBaseUrl { get; set; }

    /// <summary>
    /// Custom tags
    /// </summary>
    public string[] Tags { get; set; } = [];
}
