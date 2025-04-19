using TestBucket.Runner.Poller;
using TestBucket.Runner.Registration;
using TestBucket.Runner.Runners;
using TestBucket.Runner.Settings;

namespace TestBucket.Runner.Extensions;

public static class RunnerAppServiceExtensions
{
    public static IServiceCollection AddRunnerServices(this IServiceCollection services)
    {
        services.AddSingleton<SettingsManager>();
        services.AddHostedService<GetJobPoller>();
        services.AddHostedService<RegisterRunner>();
        services.AddHttpClient<TestBucketApiClient>((configureClient =>
        {
            configureClient.Timeout = TimeSpan.FromSeconds(120);
        }));
        return services;
    }
}
