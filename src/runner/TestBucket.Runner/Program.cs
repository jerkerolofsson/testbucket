// See https://aka.ms/new-console-template for more information
using MudBlazor.Services;

using TestBucket.Runner;
using TestBucket.Runner.Extensions;
using TestBucket.Runner.Runners;

//var workingDirectory = System.IO.Path.GetDirectoryName(".") ?? ".";

//var script = new Script
//{
//    RunnerType = "powershell",
//    Text = "echo \"Hello ${env:WHO}\"\nls",
//    WorkingDirectory = workingDirectory
//};
//script.EnvironmentVariables.Add("WHO", "PowerSHELL");

//var result = await ScriptRunnerFactory.RunAsync(script, new ConsoleObserver(), default);
//Console.WriteLine($"Success {result.Success}, ExitCode={result.ExitCode}");
//Console.WriteLine("----------------------------------------------");

//var scriptCmd = new Script
//{
//    RunnerType = "cmd",
//    Text = "echo Hello cmd.exe",
//    WorkingDirectory = workingDirectory
//};

//var resultCmd = await ScriptRunnerFactory.RunAsync(scriptCmd, new ConsoleObserver(), default);
//Console.WriteLine($"Cmd Success {result.Success}, ExitCode={result.ExitCode}");
//Console.WriteLine("----------------------------------------------");

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddControllers();

        builder.Services.AddMudServices(config =>
        {
        });

        // Proxy services
        builder.Services.AddRunnerServices();

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