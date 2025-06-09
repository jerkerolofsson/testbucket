using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        [InlineData(BrowserType.Firefox)]
        public async Task Login_WithValidCredentials_CanLogin(string browserType)
        {
            string teamName = "Team " + DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss");
            string projectName = "Test Project " + DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss");

            await using var page = new WelcomePage(Playwright, browserType);
            await page.InitializeAsync();
            await page.CreateProjectAsync(teamName, projectName);
        }
    }
}
