namespace TestBucket.Domain.Settings.Models;

/// <summary>
/// Global settings, for all tenants
/// </summary>
public class GlobalSettings
{
    public long Id { get; set; }

    /// <summary>
    /// The default tenant when the user logs in
    /// </summary>
    public string DefaultTenant { get; set; } = "default";

    /// <summary>
    /// Symmetric key for signing
    /// </summary>
    public string? SymmetricJwtKey { get; set; }

    /// <summary>
    /// JWT issuer
    /// </summary>
    public string? JwtIssuer { get; set; }

    /// <summary>
    /// JWT audience
    /// </summary>
    public string? JwtAudience { get; set; }

    /// <summary>
    /// Keep track of the changes
    /// </summary>
    public int Revision { get; set; }

    /// <summary>
    /// Base url where the Test Bucket server is publicly accessible from
    /// </summary>
    public string? PublicEndpointUrl { get; set; }
}
