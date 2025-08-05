using CliWrap;

using TestBucket.Runner.Shared;

namespace TestBucket.Runner.Runners.Powershell
{
    public class PythonRunner : IScriptRunner
    {
        public async Task<ScriptResult> RunAsync(Script script, IScriptRunnerObserver observer, CancellationToken cancellationToken)
        {
            var name = Guid.NewGuid().ToString() + ".py";
            var tempFile = Path.Combine(script.WorkingDirectory, name);
            await File.WriteAllTextAsync(tempFile, script.Text, cancellationToken);

            try
            {
                var exe = "python";
                var result = await Cli.Wrap(exe)
                    .WithArguments((args) =>
                    {
                        args.Add(tempFile);
                    })
                    .Enrich(script)
                    .LinkObserver(observer)
                    .WithValidation(CommandResultValidation.None)
                    .ExecuteAsync();

                var scriptResult = new ScriptResult();
                scriptResult.Success = result.ExitCode == 0;
                scriptResult.ExitCode = result.ExitCode;
                return scriptResult;
            }
            finally
            {
                File.Delete(tempFile);
            }
        }
    }
}
