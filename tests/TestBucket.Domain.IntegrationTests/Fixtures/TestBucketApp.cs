using DotNet.Testcontainers.Builders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Security.Claims;
using TestBucket.Contracts;
using TestBucket.Contracts.Identity;
using TestBucket.Data;
using TestBucket.Data.Migrations;
using TestBucket.Domain.Identity;
using TestBucket.Domain.IntegrationTests.Fixtures;
using TestBucket.Domain.Settings.Models;
using Testcontainers.PostgreSql;
using Xunit;
using Xunit.Sdk;

[assembly: AssemblyFixture(typeof(TestBucketApp))]

namespace TestBucket.Domain.IntegrationTests.Fixtures
{
    public class TestBucketApp(IMessageSink Sink) : IAsyncLifetime
    {
        private IHost? _host;
        private IServiceProvider? _serviceProvider;

        /// <summary>
        /// Used to control the time
        /// </summary>
        internal FakeTimeProvider TimeProvider { get; } = new FakeTimeProvider(new DateTimeOffset(2025,5,21,0,0,0,0,TimeSpan.Zero));

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
        private PostgreSqlContainer? _postgresContainer;

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
        /// Application services
        /// </summary>
        public IServiceProvider Services => _serviceProvider ?? throw new InvalidOperationException("Not inintialized!");

        public async ValueTask DisposeAsync()
        {
            if (_host is not null)
            {
                _host.Dispose();
                _host = null;
            }

            if (_postgresContainer is not null)
            {
                await _postgresContainer.DisposeAsync();
            }
        }

        private async Task StartPostgresAsync()
        {
            PostgreSqlBuilder builder = new PostgreSqlBuilder();
            builder.WithDatabase("tb-int");

            _postgresContainer = builder.Build();

            // Start the container.
            await _postgresContainer.StartAsync()
              .ConfigureAwait(false);
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

            await StartPostgresAsync();

            var builder = new HostBuilder();
            
            builder.ConfigureServices(services =>
            {
                var seedConfiguration = new SeedConfiguration
                {
                    Tenant = Environment.GetEnvironmentVariable(TestBucketEnvironmentVariables.TB_DEFAULT_TENANT),
                    Email = Environment.GetEnvironmentVariable(TestBucketEnvironmentVariables.TB_ADMIN_USER),
                    SymmetricKey = Environment.GetEnvironmentVariable(TestBucketEnvironmentVariables.TB_JWT_SYMMETRIC_KEY),
                    Issuer = Environment.GetEnvironmentVariable(TestBucketEnvironmentVariables.TB_JWT_ISS),
                    Audience = Environment.GetEnvironmentVariable(TestBucketEnvironmentVariables.TB_JWT_AUD),
                    AccessToken = Environment.GetEnvironmentVariable(TestBucketEnvironmentVariables.TB_ADMIN_ACCESS_TOKEN),
                    PublicEndpointUrl = Environment.GetEnvironmentVariable(TestBucketEnvironmentVariables.TB_PUBLIC_ENDPOINT),
                    Password = Environment.GetEnvironmentVariable(TestBucketEnvironmentVariables.TB_ADMIN_PASSWORD),
                };
                services.AddSingleton(seedConfiguration);

                services.AddHostedService<MigrationService>();

                services.AddLogging((builder) => builder.AddXUnit(Sink));

                services.AddDbContext<ApplicationDbContext>(options => 
                { 
                    options.UseNpgsql(_postgresContainer!.GetConnectionString(), builder =>
                    {
                        builder.ConfigureDataSource(dataSource =>
                        {
                            dataSource.EnableDynamicJson();
                        });
                    }); 
                });
                services.AddDbContextFactory<ApplicationDbContext>();

                services.AddMemoryCache();
                services.AddDataServices();
                services.AddDomainServices();
                services.AddSingleton<TimeProvider>(TimeProvider);
            });
            _host = builder.Build();

            _serviceProvider = _host.Services;

            ThreadPool.QueueUserWorkItem((s) =>
            {
                try
                {
                    _host.Run();
                }
                catch(Exception ex)
                {
                    var logger = _serviceProvider.GetRequiredService<ILogger<TestBucketApp>>();
                    logger.LogError(ex, "Failure running host");
                }
            });

            // Wait for migrations
            MigrationReadyWaiter.Wait();
        }
    }
}
