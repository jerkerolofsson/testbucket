using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Scalar.AspNetCore;
using TestBucket.Servers.AdbProxy.Controllers;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();
        builder.Services.AddOpenApi();

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddControllers()
            .PartManager.ApplicationParts.Add(new AssemblyPart(typeof(ResourceController).Assembly));

        builder.Services.AddMudServices(config =>
        {
        });
        builder.Services.AddTestBucketResourceServer();

        // Proxy services
        builder.Services.AddAdbProxyServices();

        var app = builder.Build();

        app.UseAntiforgery();
        app.UseHttpsRedirection();

        app.MapOpenApi();
        app.MapScalarApiReference();

        app.MapStaticAssets();
        app.MapControllers();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}