using Microsoft.DependencyInject.Extensions;

using TestBucket.AdbProxy.Appium;
using TestBucket.AdbProxy.DeviceHandling;
using TestBucket.AdbProxy.Inform;
using TestBucket.AdbProxy.Proxy;

namespace Microsoft.Extensions.DependencyInjection;
public static class AdbProxyServiceExtensions
{
    public static IServiceCollection AddAdbProxyServices(this IServiceCollection services)
    {

        services.AddSingleton<IAdbDeviceRepository, AdbDeviceRepository>();
        services.AddSingleton<IPortGenerator, AdbProxyServerPortGenerator>();
        services.AddSingleton<IDeviceInformer, DeviceInformer>();
        services.AddHostedService<AdbDeviceIndexingService>();
        services.AddDockerCompose();
        services.AddHostedService<AppiumDockerCleaner>();

        return services;
    }
}
