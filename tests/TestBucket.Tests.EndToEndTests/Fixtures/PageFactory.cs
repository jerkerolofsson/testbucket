using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Tests.EndToEndTests.Pages;

namespace TestBucket.Tests.EndToEndTests.Fixtures
{
    internal class PageFactory
    {
        public static async ValueTask<T> CreateAsync<T>(IServiceProvider serviceProvider, PlaywrightFixture fixture, string browserType, BrowserTypeLaunchOptions? options = null) where T : BasePage
        {
            var context = await fixture.CreateBrowserContextAsync(browserType);
            context.Context.SetDefaultNavigationTimeout(20_000);

            var page = await context.Context.NewPageAsync();

            var pageModel = ActivatorUtilities.CreateInstance<T>(serviceProvider, context, page);
            return pageModel;
        }
    }
}
