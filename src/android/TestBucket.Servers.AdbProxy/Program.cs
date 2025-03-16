
using TestBucket.AdbProxy.DeviceHandling;
using TestBucket.AdbProxy.Proxy;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();

        //HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddControllers();

        builder.Services.AddMudServices(config =>
        {
        });

        // Proxy services
        builder.Services.AddAdbProxyServices();

        var app = builder.Build();

        app.UseAntiforgery();
        app.UseHttpsRedirection();

        app.MapStaticAssets();
        app.MapControllers();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}