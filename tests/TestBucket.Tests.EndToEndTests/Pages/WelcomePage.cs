using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Tests.EndToEndTests.Fixtures;

namespace TestBucket.Tests.EndToEndTests.Pages
{
    internal class WelcomePage : AuthentictedPage
    {
        public WelcomePage(PlaywrightFixture Fixture, string BrowserType) : base(Fixture, BrowserType)
        {
        }

        internal async Task CreateProjectAsync(string teamName, string projectName)
        {
            // Click the welcome button
            await Page.GetByTestId("welcome-add-team").ClickAsync();
            await Page.GetByTestId("add-team-dialog").WaitForAsync();
            await Page.GetByTestId("name").FillAsync(teamName);
            await Page.GetByTestId("ok").ClickAsync();

            await Task.Delay(10000);

            await Page.GetByTestId("welcome-add-project").ClickAsync();
            await Page.GetByTestId("add-project-dialog").WaitForAsync();
            await Page.GetByTestId("name").FillAsync(projectName);
            await Page.GetByTestId("ok").ClickAsync();

            await Task.Delay(10000);
        }
    }
}
