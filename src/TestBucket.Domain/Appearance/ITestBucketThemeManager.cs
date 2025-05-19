
using TestBucket.Domain.Appearance.Models;

namespace TestBucket.Domain.Appearance;
public interface ITestBucketThemeManager
{
    /// <summary>
    /// Returns the theme for hte current user
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    Task<TestBucketBaseTheme> GetCurrentThemeAsync(ClaimsPrincipal user);
    Task<TestBucketBaseTheme> GetThemeByNameAsync(string? name);
    Task<string> GetThemedStylesheetAsync(ClaimsPrincipal user);
}