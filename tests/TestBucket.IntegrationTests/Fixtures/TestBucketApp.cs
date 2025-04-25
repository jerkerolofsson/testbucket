using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using TestBucket.Contracts;
using TestBucket.Contracts.Identity;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Settings.Models;
using TestBucket.IntegrationTests.Fixtures;
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

            //// Arrange
            var builder = await DistributedApplicationTestingBuilder
                .CreateAsync<Projects.TestBucket_AppHost>(TestContext.Current.CancellationToken);

            builder.Services.ConfigureHttpClientDefaults(clientBuilder =>
            {
                clientBuilder.AddStandardResilienceHandler();
            });

            // To output logs to the xUnit.net ITestOutputHelper, 
            builder.Services.AddLogging((builder) => builder.AddXUnit(Sink));

            var app = await builder.BuildAsync(TestContext.Current.CancellationToken);
            await app.StartAsync(TestContext.Current.CancellationToken);

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            await app.ResourceNotifications.WaitForResourceHealthyAsync(
                "testbucket",
                cts.Token);
            _app = app;
        }
    }
}
