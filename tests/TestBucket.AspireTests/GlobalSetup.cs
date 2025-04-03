// Here you could define global logic that would affect all tests

// You can use attributes at the assembly level to apply to all tests in the assembly

using Aspire.Hosting;
using System.Security.Claims;
using TestBucket.Contracts.Identity;
using TestBucket.Domain.Settings.Models;

[assembly: Retry(3)]
[assembly: System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]

namespace TestBucket.AspireTests.TestProject
{
    public class GlobalSetup
    {
        public static DistributedApplication? App { get; private set; }
        public static ResourceNotificationService? NotificationService { get; private set; }

        public static SeedConfiguration? SeedConfiguration { get; set; }

        private static SeedConfiguration SetupUser()
        {
            // Setup users and keys for seeding
            var testCredentials = new SeedConfiguration
            {
                Email = "admin@admin.com",
                Tenant = "jerkerolofsson",
                SymmetricKey = "01234567890123456789012345678901234567890123456789",
                Issuer = "testbucket",
                Audience = "testbucket"
            };
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, testCredentials.Email),
                new Claim("tenant", testCredentials.Tenant),
            };

            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            // For development, we inject a symmetric key and generate an API key to use so services can communicate
            var apiKeyGenerator = new ApiKeyGenerator(testCredentials.SymmetricKey, testCredentials.Issuer, testCredentials.Audience);
            testCredentials.AccessToken = apiKeyGenerator.GenerateAccessToken(principal, DateTime.UtcNow.AddDays(100));
            return testCredentials;
        }

        [Before(TestSession)]
        public static async Task SetUp()
        {
            // Arrange
            var timeoutCancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(60)).Token;

            SeedConfiguration = SetupUser();

            var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.TestBucket>([], (options, configureBuilder) =>
            {
                options.AllowUnsecuredTransport = true;

            }, timeoutCancellationToken);
            appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
            {
                clientBuilder.AddStandardResilienceHandler();
            });
            appHost.Services.AddSingleton<SeedConfiguration>();

            App = await appHost.BuildAsync();
            NotificationService = App.Services.GetRequiredService<ResourceNotificationService>();
            await App.StartAsync();
        }

        [After(TestSession)]
        public static void CleanUp()
        {
            Console.WriteLine("...and after!");
        }
    }
}