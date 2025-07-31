using TestBucket.ResosurceServer.Registry;
using TestBucket.ResourceServer.Services.Inform;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceExtensions
{
    public static IServiceCollection AddTestBucketResourceServer(this IServiceCollection services)
    {
        services.AddSingleton<IResourceRegistry, ResourceRegistry>();
        services.AddSingleton<IResourceInformer, ResourceInformer>();
        return services;
    }
}
