using Playwright.Axe;
using TestBucket.Tests.EndToEndTests.Fixtures;

namespace TestBucket.Tests.EndToEndTests.Pages
{
    internal class LoginPage : BasePage
    {

        public LoginPage(BrowserTestContext context, IPage page) : base(context, page)
        {

        }

        public async Task OpenAsync()
        {
            await _page.GotoAsync(Fixture.HttpsBaseUrl + "Login");
        }

        public async Task LoginAsync()
        {
            await LoginAsync(Fixture.Configuration.Email, Fixture.Configuration.Password);
        }

        /// <summary>
        /// Returns true if login was successful
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        internal async Task<bool> LoginAsync(string? email, string? password)
        {
            await OpenAsync();

            if (email is not null)
            {
                await _page.GetByTestId("email").FillAsync(email);
            }
            if (password is not null)
            {
                await _page.GetByTestId("password").FillAsync(password);
            }

            await _page.GetByTestId("login").ClickAsync();

            await WaitForAnyAsync(_page, "main-toolbar", "login-error");

            if (await _page.GetByTestId("main-toolbar").IsVisibleAsync())
            {
                // Success
                return true;
            }

            return false;
        }

        internal async Task<AxeResults> RunAxeOnLoginAsync()
        {
            await _page.GotoAsync(Fixture.HttpsBaseUrl + "Login");
            await _page.GetByTestId("email").WaitForAsync();

            AxeResults axeResults = await _page.RunAxe();
            return axeResults;
        }

    }
}
