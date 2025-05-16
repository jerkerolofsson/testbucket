using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TestBucket.Controllers.Api;
using TestBucket.Domain.Appearance;
using TestBucket.Domain.Identity;

namespace TestBucket.Components.Appearance.ApiControllers;

[ApiController]
public class CssController : ProjectApiControllerBase
{
    private readonly ITestBucketThemeManager _themeManager;

    public CssController(ITestBucketThemeManager themeManager)
    {
        _themeManager = themeManager;
    }

    [Authorize("ApiKeyOrBearer")]
    [HttpGet("/api/appearance/usertheme.css")]
    public async Task<IActionResult> GetUserThemeCssAsync()
    {
        var css = await _themeManager.GetThemedStylesheetAsync(User);
        return new ContentResult
        {
            Content = css,
            ContentType = "text/css",
            StatusCode = 200
        };
    }

}
