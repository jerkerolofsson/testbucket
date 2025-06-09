using k8s.KubeConfigModels;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Tests.EndToEndTests.Fixtures;

namespace TestBucket.Tests.EndToEndTests.Pages
{
    public abstract class AuthentictedPage(PlaywrightFixture Fixture, string BrowserType) : IAsyncDisposable
    {
        private IBrowser? _browser;
        private IBrowserContext? _context;
        private IPage? _page;

        public IPage Page => _page ?? throw new InvalidOperationException("Page is not initialized. Call LoginAsync first.");

        public async ValueTask AddScreenshotAsync(string name)
        {
            if (_page is not null)
            {
                using var tempDir = new TempDir();
                var screenshotPath = Path.Combine(tempDir.TempPath, "screenshot.png");
                await _page.ScreenshotAsync(new PageScreenshotOptions
                {
                    Path = screenshotPath
                });
                var bytes = File.ReadAllBytes(screenshotPath);
                TestContext.Current.AddAttachment($"{name}.png", bytes, "image/png");
            }
            else
            {
                throw new InvalidOperationException("Page is not initialized. Cannot take screenshot.");
            }
        }

        public async ValueTask InitializeAsync()
        {
            _browser = await Fixture.CreateBrowserAsync(BrowserType);
            _context = await _browser.NewContextAsync(new() { IgnoreHTTPSErrors = true });

            // Do login
            var page = new LoginPage(Fixture, _browser);
            var loginSuccess = await page.LoginAsync(Fixture.Configuration.Email, Fixture.Configuration.Password, _context);
            _page = page.Page;
            if (!loginSuccess)
            {
                if (_page is not null)
                {
                    await AddScreenshotAsync("LoginFailed");
                }

                throw new Exception("Login failed");
            }
        }
        public async ValueTask DisposeAsync()
        {
            if (_context is not null)
            {
                await _context.CloseAsync();
                await _context.DisposeAsync();
            }
            if (_browser is not null)
            {
                await _browser.CloseAsync();
                await _browser.DisposeAsync();
            }
           
        }

    }
}
