using TestBucket.Domain.Appearance.Models;

namespace TestBucket.Components.Shared.Themeing.Models;

public record class ApplicationState(ClaimsPrincipal User)
{
    public required bool IsDarkMode { get; set; } 
    public required TestBucketBaseTheme Theme { get; set; }
}
