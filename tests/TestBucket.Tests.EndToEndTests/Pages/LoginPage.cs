using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Tests.EndToEndTests.Fixtures;

namespace TestBucket.Tests.EndToEndTests.Pages
{
    internal class LoginPage(PlaywrightFixture Fixture)
    {
        internal async Task LoginAsync(string email, string password)
        {
            var page = await Fixture.Browser.NewPageAsync();
            await page.GotoAsync(Fixture.HttpsBaseUrl + "/Login");

            await page.FillAsync("#Input.Email", email);
            await page.FillAsync("#Input.Password", password);

            await page.ClickAsync("#Input.Login");

            await Task.Delay(30000);
            // #Input.Email
            // #Input.Password
        }
    }
}
