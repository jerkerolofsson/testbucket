using TestBucket.Tests.EndToEndTests.Fixtures;

namespace TestBucket.Tests.EndToEndTests.Pages
{
    public abstract class BasePage : IAsyncDisposable
    {
        private readonly BrowserTestContext _context;
        protected IPage _page = null!;

        public PlaywrightFixture Fixture => _context.Fixture;
        public IBrowser Browser => _context.Browser;
        public IBrowserContext Context => _context.Context;

        public IPage Page
        {
            get
            {
                return _page ?? throw new InvalidOperationException("Page is not initialized. Call LoginAsync first.");
            }
            set
            {
                _page = value;
            }
        }

        public BasePage(BrowserTestContext context, IPage page)
        {
            _context = context;
            _page = page;
        }
        protected BasePage(BasePage otherPage)
        {
            _context = otherPage._context;
            _page = otherPage._page;
        }

        public async Task NewPageAsync()
        {
            this._page = await Context.NewPageAsync();
        }

        public async Task<IElementHandle?> WaitForAnyAsync(IPage page, params string[] testIds)
        {
            var tasks = testIds.Select(testId =>
                page.GetByTestId(testId).ElementHandleAsync(new() { Timeout = 10000 }) // adjust timeout as needed
            ).ToArray();

            var completed = await Task.WhenAny(tasks);
            try
            {
                return await completed;
            }
            catch
            {
                var testIdsAsString = string.Join(',', testIds);
                await AddScreenshotAsync($"Error waiting for {testIdsAsString}");
                throw;
            }
        }

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
        public async ValueTask DisposeAsync()
        {
            if (_context?.Context is not null)
            {
                await _context.Context.CloseAsync();
                await _context.Context.DisposeAsync();
            }
            if (_context?.Browser is not null)
            {
                await _context.Browser.CloseAsync();
                await _context.Browser.DisposeAsync();
            }
           
        }

    }
}
