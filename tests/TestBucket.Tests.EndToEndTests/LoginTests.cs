using Microsoft.Playwright;
using System.Threading.Tasks;
using TestBucket.Tests.EndToEndTests.Fixtures;
using TestBucket.Tests.EndToEndTests.Pages;
using TestBucket.Traits.Xunit;
using TestBucket.Traits.Xunit.Enrichment;

namespace TestBucket.Tests.EndToEndTests
{
    [Component("Identity")]
    [EndToEndTest]
    [EnrichedTest]
    public class LoginTests(PlaywrightFixture Playwright) : IClassFixture<PlaywrightFixture>
    {
        [Theory]
        [InlineData(BrowserType.Chromium)]
        [InlineData(BrowserType.Webkit)]
        [InlineData(BrowserType.Firefox)]
        public async Task Login_WithValidCredentials_CanLogin(string browserType)
        {
            await using var browser = await Playwright.CreateBrowserAsync(browserType);
            var loginPage = new LoginPage(Playwright, browser);
            var success = await loginPage.LoginAsync(Playwright.Configuration.Email, Playwright.Configuration.Password);
            Assert.True(success, "Login should be successful with valid credentials");
        }

        [Theory]
        [InlineData(BrowserType.Chromium)]
        [InlineData(BrowserType.Webkit)]
        [InlineData(BrowserType.Firefox)]
        public async Task Login_WithInvalidCredentials_CannotLogin(string browserType)
        {
            await using var browser = await Playwright.CreateBrowserAsync(browserType);
            var loginPage = new LoginPage(Playwright, browser);
            var success = await loginPage.LoginAsync(Playwright.Configuration.Email, Playwright.Configuration.Password + "-not");
            Assert.False(success, "Login should not be successful with invalid credentials");
        }
    }
}
