using Microsoft.AspNetCore.Mvc.ApplicationParts;

using MudBlazor.Services;

using Scalar.AspNetCore;

using TestBucket.Servers.AdbProxy.Controllers;
using TestBucket.Servers.NodeResourceServer.Components;
using TestBucket.Servers.NodeResourceServer.Services.Inform;
using TestBucket.Servers.NodeResourceServer.Services.Playwright;

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

        builder.Services.AddHostedService<PlaywrightRunner>();
        builder.Services.AddTransient<IServiceInformer, ServiceInformer>();
        

        // Proxy services

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