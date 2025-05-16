
using TestBucket.Domain.Appearance.Models;

namespace TestBucket.Domain.Appearance;
public interface ITestBucketThemeManager
{
    Task<TestBucketBaseTheme> GetThemeByNameAsync(string? name);
    Task<string> GetThemedStylesheetAsync(ClaimsPrincipal user);
}