using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Security.Claims;
using TestBucket.Contracts;
using TestBucket.Contracts.Identity;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Settings.Models;
using TestBucket.Sdk.Client;
using Xunit.Sdk;

[assembly: AssemblyFixture(typeof(TestBucketApp))]


namespace TestBucket.IntegrationTests.Fixtures
{
    public class TestBucketApp(IMessageSink Sink) : IAsyncLifetime
    {
        private DistributedApplication? _app;

        public string Tenant => _configuration.Tenant ?? throw new InvalidOperationException("Invalid SeedConfiguration in fixture");
        public SeedConfiguration Configuration => _configuration;

        private SeedConfiguration _configuration = new SeedConfiguration
        {
            Tenant  = "jerkerolofsson",
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

        /// <summary>
        /// Generates an access token for the specified user
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public string GenerateAccessToken(ClaimsPrincipal principal)
        {
            var apiKeyGenerator = new ApiKeyGenerator(_configuration.SymmetricKey!, _configuration.Issuer!, _configuration.Audience!);
            return apiKeyGenerator.GenerateAccessToken(principal, DateTime.UtcNow.AddDays(100));
        }

        /// <summary>
        /// SDK api client
        /// </summary>
        public TestBucketClient Client => new TestBucketClient(CreateAdministratorHttpClient());

        public HttpClient CreateClient(ClaimsPrincipal principal)
        {
            var client = _app?.CreateHttpClient("testbucket") ?? throw new InvalidOperationException("Not ready - not initialized");
            var accessToken = GenerateAccessToken(principal);
            client.DefaultRequestHeaders.Add("ApiKey", accessToken);
            return client;
        }

        /// <summary>
        /// Creates an authenticated http client
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public HttpClient CreateAdministratorHttpClient()
        {
            var client = _app?.CreateHttpClient("testbucket") ?? throw new InvalidOperationException("Not ready - not initialized");
            var accessToken = GenerateAccessToken(SiteAdministrator);
            client.DefaultRequestHeaders.Add("ApiKey", accessToken);
            return client;
        }

        public async ValueTask DisposeAsync()
        {
            if (_app is not null)
            {
                await _app.DisposeAsync();
            }
        }

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
            var logger = app.Services.GetRequiredService<ILogger<TestBucketApp>>();

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
        }
    }
}
