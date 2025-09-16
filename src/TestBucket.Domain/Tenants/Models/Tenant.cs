namespace TestBucket.Domain.Tenants.Models;
public record class Tenant
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? IconUrl { get; set; }

    /// <summary>
    /// CI/CD access token
    /// </summary>
    public string? CiCdAccessToken { get; set; }

    /// <summary>
    /// CI/CD access token expiry
    /// </summary>
    public DateTimeOffset? CiCdAccessTokenExpires { get; set; }

    public bool CanRegisterNewUsers { get; set; }
    public bool RequireConfirmedAccount { get; set; }


    // Navigation
    public IEnumerable<TestProject>? TestProjects { get; set; }
}
