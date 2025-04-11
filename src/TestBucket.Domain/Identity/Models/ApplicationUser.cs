using Microsoft.AspNetCore.Identity;

namespace TestBucket.Domain.Identity.Models;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public string TenantId { get; set; } = "no-tenant";

    /// <summary>
    /// Profile image uri
    /// </summary>
    public string? ProfileImageUri { get; set; }


    // Navigation
    public virtual IEnumerable<ApplicationUserApiKey>? ApplicationUserApiKeys { get; set; }
}

