// Here you could define global logic that would affect all tests

// You can use attributes at the assembly level to apply to all tests in the assembly

using Aspire.Hosting;

[assembly: Retry(3)]
[assembly: System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]

namespace TestBucket.AspireTests.TestProject
{
    public class GlobalSetup
    {
        public static DistributedApplication? App { get; private set; }
        public static ResourceNotificationService? NotificationService { get; private set; }


        [Before(TestSession)]
        public static async Task SetUp()
        {
            // Arrange
            var timeoutCancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(60)).Token;

            var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.TestBucket>([], (options, configureBuilder) =>
            {
                options.AllowUnsecuredTransport = true;

            }, timeoutCancellationToken);
            appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
            {
                clientBuilder.AddStandardResilienceHandler();
            });

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