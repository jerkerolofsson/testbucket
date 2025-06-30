using Playwright.Axe;
using TestBucket.Tests.EndToEndTests.Fixtures;

namespace TestBucket.Tests.EndToEndTests.Pages
{
    internal class LoginPage(PlaywrightFixture Fixture, IBrowser Browser)
    {
        private IPage? _page;

        public IPage Page => _page ?? throw new InvalidOperationException("Page is not initialized. Call LoginAsync first.");

        /// <summary>
        /// Returns true if login was successful
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        internal async Task<bool> LoginAsync(string? email, string? password)
        {
            await using var context = await Browser.NewContextAsync(new() { IgnoreHTTPSErrors = true });
            return await LoginAsync(email, password, context);
        }

        internal async Task<AxeResults> RunAxeOnLoginAsync()
        {
            await using var context = await Browser.NewContextAsync(new() { IgnoreHTTPSErrors = true });
            _page = await context.NewPageAsync();
            await _page.GotoAsync(Fixture.HttpsBaseUrl + "Login");
            await _page.GetByTestId("email").WaitForAsync();

            AxeResults axeResults = await _page.RunAxe();
            return axeResults;
        }

        internal async Task<bool> LoginAsync(string? email, string? password, IBrowserContext context)
        {
            _page = await context.NewPageAsync();
            await _page.GotoAsync(Fixture.HttpsBaseUrl + "Login");

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

        public static async Task<IElementHandle?> WaitForAnyAsync(IPage page, params string[] testIds)
        {
            var tasks = testIds.Select(testId =>
                page.GetByTestId(testId).ElementHandleAsync(new() { Timeout = 10000 }) // adjust timeout as needed
            ).ToArray();

            var completed = await Task.WhenAny(tasks);
            return await completed;
        }
    }
}
