using System.Threading.Tasks;
using TestBucket.Tests.EndToEndTests.Fixtures;
using TestBucket.Tests.EndToEndTests.Pages;
using TestBucket.Traits.Xunit;

namespace TestBucket.Tests.EndToEndTests
{
    [Component("Identity")]
    [EndToEndTest]
    [EnrichedTest]
    public class LoginTests(PlaywrightFixture Playwright) : IClassFixture<PlaywrightFixture>
    {
        [Fact]
        public async Task Login_WithValidCredentials_CanLogin()
        {
            var loginPage = new LoginPage(Playwright);
            await loginPage.LoginAsync(Playwright.Configuration.Email, Playwright.Configuration.Password);
        }
    }
}
