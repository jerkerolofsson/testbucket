using TestBucket.Tests.EndToEndTests.Fixtures;
using TestBucket.Tests.EndToEndTests.Pages;
using TestBucket.Traits.Xunit;

namespace TestBucket.Tests.EndToEndTests.Welcome
{
    [Feature("Welcome Experience")]
    [Component("Project")]
    [EndToEndTest]
    [EnrichedTest]
    public class WelcomeProjectTests(PlaywrightFixture Playwright) : IClassFixture<PlaywrightFixture>
    {
        [Theory]
        [InlineData(BrowserType.Chromium)]
        [InlineData(BrowserType.Firefox)]
        public async Task Login_WithValidCredentials_CanLogin(string browserType)
        {
            string teamName = "Team " + DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss");
            string projectName = "Test Project " + DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss");

            await using var loginPage = await Playwright.OpenAsync(browserType);
            await loginPage.LoginAsync();

            await using var page = new WelcomePage(loginPage);
            await page.CreateProjectAsync(teamName, projectName);
        }
    }
}
