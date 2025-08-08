/// <summary>
/// Integration tests for project export functionality, verifying sensitive details handling.
/// </summary>
/// <param name="Fixture">The test fixture providing project context and services.</param>
public class ProjectExportTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
{
    /// <summary>
    /// Verifies that an exported project does not include sensitive details (ApiKey and AccessToken) when requested.
    /// </summary>
    /// <returns>A task representing the asynchronous test operation.</returns>
    [Fact]
    public async Task ExportProject_WithoutSensitiveDetails_ApiKeyAndAccessTokenNotIncluded()
    {
        // ...existing code...
    }

    /// <summary>
    /// Verifies that an exported project does include sensitive details (ApiKey and AccessToken) when requested.
    /// </summary>
    /// <returns>A task representing the asynchronous test operation.</returns>
    [Fact]
    public async Task ExportProject_WithSensitiveDetails_ApiKeyAndAccessTokenNotIncluded()
    {
        // ...existing code...
    }

    /// <summary>
    /// Creates a new ExternalSystem integration instance for testing.
    /// </summary>
    /// <returns>A new ExternalSystem object with test data.</returns>
    private static ExternalSystem CreateIntegration()
    {
        // ...existing code...
    }
}
