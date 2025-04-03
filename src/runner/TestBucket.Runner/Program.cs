// See https://aka.ms/new-console-template for more information
using TestBucket.Runner;

var workingDirectory = System.IO.Path.GetDirectoryName(".") ?? ".";

var script = new Script
{
    RunnerType = "powershell",
    Text = "echo \"Hello ${env:WHO}\"\nls",
    WorkingDirectory = workingDirectory
};
script.EnvironmentVariables.Add("WHO", "PowerSHELL");

var result = await ScriptRunnerFactory.RunAsync(script, new ConsoleObserver(), default);
Console.WriteLine($"Success {result.Success}, ExitCode={result.ExitCode}");
Console.WriteLine("----------------------------------------------");

var scriptCmd = new Script
{
    RunnerType = "cmd",
    Text = "echo Hello cmd.exe",
    WorkingDirectory = workingDirectory
};

var resultCmd = await ScriptRunnerFactory.RunAsync(scriptCmd, new ConsoleObserver(), default);
Console.WriteLine($"Cmd Success {result.Success}, ExitCode={result.ExitCode}");
Console.WriteLine("----------------------------------------------");