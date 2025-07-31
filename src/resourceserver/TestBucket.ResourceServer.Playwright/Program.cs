using Microsoft.Extensions.DependencyInjection;

using TestBucket.ResourceServer.Playwright.Components;
using TestBucket.ResourceServer.Playwright.Services;

namespace TestBucket.ResourceServer.Playwright;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();
        builder.AddServiceDefaults();

        builder.Services
            .AddHttpClient()
            .AddTestBucketResourceServer()
            .AddDockerCompose();
        builder.Services.AddHostedService<PlaywrightDockerService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseAntiforgery();

        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}
