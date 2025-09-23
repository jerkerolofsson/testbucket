using CliWrap;

using TestBucket.Runner.Shared;

namespace TestBucket.Runner.Runners.Powershell
{
    public class CSharpRunner : IScriptRunner
    {
        private static string? _exe;
        
        public async Task<ScriptResult> RunAsync(Script script, IScriptRunnerObserver observer, CancellationToken cancellationToken)
        {
            var name = Guid.NewGuid().ToString() + ".cs";
            var tempFile = Path.Combine(script.WorkingDirectory, name);
            await File.WriteAllTextAsync(tempFile, script.Text, cancellationToken);

            try
            {
                string exe = ResolveExecutable();

                var result = await Cli.Wrap(exe)
                    .WithArguments((args) =>
                    {
                        args.Add("run");
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

        internal static string ResolveExecutable()
        {
            if (_exe != null)
            {
                return _exe;
            }
            _exe = "dotnet";
            return _exe;
        }
    }
}
