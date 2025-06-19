using TestBucket.ResosurceServer.Registry;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceExtensions
{
    public static IServiceCollection AddTestBucketResourceServer(this IServiceCollection services)
    {
        services.AddSingleton<IResourceRegistry, ResourceRegistry>();
        return services;
    }
}
