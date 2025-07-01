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
    public class LoginTests(PlaywrightFixture Playwright, ITestOutputHelper testOutputHelper) : IClassFixture<PlaywrightFixture>
    {
        [FunctionalTest]
        [Theory]
        [InlineData(BrowserType.Chromium)]
        [InlineData(BrowserType.Webkit)]
        [InlineData(BrowserType.Firefox)]
        public async Task Login_WithValidCredentials_CanLogin(string browserType)
        {
            await using var loginPage = await Playwright.OpenAsync(browserType);
            var success = await loginPage.LoginAsync(Playwright.Configuration.Email, Playwright.Configuration.Password);
            Assert.True(success, "Login should be successful with valid credentials");
        }

        [FunctionalTest]
        [Theory]
        [InlineData(BrowserType.Chromium)]
        [InlineData(BrowserType.Webkit)]
        [InlineData(BrowserType.Firefox)]
        public async Task Login_WithInvalidCredentials_CannotLogin(string browserType)
        {
            await using var loginPage = await Playwright.OpenAsync(browserType);
            var success = await loginPage.LoginAsync(Playwright.Configuration.Email, Playwright.Configuration.Password + "-not");
            Assert.False(success, "Login should not be successful with invalid credentials");
        }


        [UsabilityTest]
        [Fact]
        public async Task Login_Accessibility()
        {
            await using var loginPage = await Playwright.OpenAsync(BrowserType.Chromium);
            var results = await loginPage.RunAxeOnLoginAsync();
            await Task.Delay(15000, TestContext.Current.CancellationToken);
            foreach(var violation in results.Violations)
            {
                testOutputHelper.WriteLine($"Violation: {violation.Id}: {violation.Description}");
            }
            Assert.Empty(results.Violations);
        }

    }
}
