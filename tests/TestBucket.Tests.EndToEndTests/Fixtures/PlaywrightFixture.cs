using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using System.Diagnostics;
using System.Security.Claims;
using TestBucket.Contracts;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Settings.Models;
using Xunit.Sdk;

namespace TestBucket.Tests.EndToEndTests.Fixtures
{
    public class PlaywrightFixture(IMessageSink Sink) : IAsyncLifetime
    {
        private DistributedApplication? _app;
        private IPlaywright? _playwright;
        private IBrowser? _browser;
        public string Tenant => _configuration.Tenant ?? throw new InvalidOperationException("Invalid SeedConfiguration in fixture");
        public SeedConfiguration Configuration => _configuration;

        public string HttpsBaseUrl => _app?.GetEndpoint("testbucket", "https")?.ToString() ?? throw new InvalidOperationException("Not initialized yet");
        public IBrowser Browser => _browser ?? throw new InvalidOperationException("Not initialized yet");

        private SeedConfiguration _configuration = new SeedConfiguration
        {
            Tenant = "jerkerolofsson",
            Email = "admin@admin.com",
            Password = "Password@123",
            SymmetricKey = "01234567890123456789012345678901234567890123456789",
            Audience = "testbucket",
            Issuer = "testbucket"
        };

        /// <summary>
        /// A principal with admin access
        /// </summary>
        public ClaimsPrincipal SiteAdministrator => Impersonation.Impersonate(_configuration.Tenant);


        public async ValueTask InitializeAsync()
        {
            // Set environment variables to create the initial seed 
            Environment.SetEnvironmentVariable(TestBucketEnvironmentVariables.TB_DEFAULT_TENANT, _configuration.Tenant);
            Environment.SetEnvironmentVariable(TestBucketEnvironmentVariables.TB_JWT_SYMMETRIC_KEY, _configuration.SymmetricKey);
            Environment.SetEnvironmentVariable(TestBucketEnvironmentVariables.TB_JWT_ISS, _configuration.Issuer);
            Environment.SetEnvironmentVariable(TestBucketEnvironmentVariables.TB_JWT_AUD, _configuration.Audience);
            Environment.SetEnvironmentVariable(TestBucketEnvironmentVariables.TB_ADMIN_USER, _configuration.Email);
            Environment.SetEnvironmentVariable(TestBucketEnvironmentVariables.TB_ADMIN_PASSWORD, _configuration.Password);
            Environment.SetEnvironmentVariable("TB_IS_INTEGRATION_TEST", "yes");

            //// Arrange
            var builder = await DistributedApplicationTestingBuilder
                .CreateAsync<Projects.TestBucket_AppHost>(TestContext.Current.CancellationToken);

            builder.Services.ConfigureHttpClientDefaults(clientBuilder =>
            {
                //clientBuilder.AddStandardResilienceHandler(configure =>
                //{
                //    configure.TotalRequestTimeout = new Microsoft.Extensions.Http.Resilience.HttpTimeoutStrategyOptions
                //    {
                //        Timeout = TimeSpan.FromSeconds(60)
                //    };
                //});
            });

            // To output logs to the xUnit.net ITestOutputHelper, 
            builder.Services.AddLogging((builder) => builder.AddXUnit(Sink));

            using var ctsBuild = new CancellationTokenSource(TimeSpan.FromSeconds(120));
            var app = await builder.BuildAsync(ctsBuild.Token);
            var logger = app.Services.GetRequiredService<ILogger<PlaywrightFixture>>();

            using var ctsStart = new CancellationTokenSource(TimeSpan.FromSeconds(120));
            logger.LogInformation("Starting..");
            await app.StartAsync(ctsStart.Token);

            var start = Stopwatch.GetTimestamp();
            try
            {
                logger.LogInformation("Waiting for healthy..");
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(240));
                await app.ResourceNotifications.WaitForResourceHealthyAsync(
                    "testbucket",
                    cts.Token);
            }
            catch (Exception ex)
            {
                var elapsed = Stopwatch.GetElapsedTime(start);
                logger.LogError(ex, "Failed to start after {elapsedSeconds}", elapsed.TotalSeconds);

                throw;
            }
            _app = app;

            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        }

        public async ValueTask DisposeAsync()
        {
            if (_app is not null)
            {
                await _app.DisposeAsync();
            }
            if (_browser is not null)
            {
                await _browser.CloseAsync();
            }
            if (_playwright is not null)
            {
                _playwright.Dispose();
            }
        }
    }
}
