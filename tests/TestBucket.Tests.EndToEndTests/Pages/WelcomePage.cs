using TestBucket.Tests.EndToEndTests.Fixtures;

namespace TestBucket.Tests.EndToEndTests.Pages
{
    internal class WelcomePage : BasePage
    {
        public WelcomePage(BrowserTestContext context, IPage page) : base(context, page)
        {

        }
        public WelcomePage(BasePage otherPage) : base(otherPage)
        {

        }

        internal async Task CreateProjectAsync(string teamName, string projectName)
        {
            // Click the welcome button
            await Page.GetByTestId("welcome-add-team").ClickAsync();
            await Page.GetByTestId("add-team-dialog").WaitForAsync();
            await Page.GetByTestId("name").FillAsync(teamName);
            await Page.GetByTestId("ok").ClickAsync();

            await Page.GetByTestId("welcome-add-project").ClickAsync();
            await Page.GetByTestId("add-project-dialog").WaitForAsync();
            await Page.GetByTestId("name").FillAsync(projectName);
            await Page.GetByTestId("ok").ClickAsync();

        }
    }
}
