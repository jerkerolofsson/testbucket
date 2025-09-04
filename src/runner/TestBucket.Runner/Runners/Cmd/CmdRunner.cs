using CliWrap;

using TestBucket.Runner.Shared;

namespace TestBucket.Runner.Runners.Cmd
{
    public class CmdRunner : IScriptRunner
    {
        public async Task<ScriptResult> RunAsync(Script script, IScriptRunnerObserver observer, CancellationToken cancellationToken)
        {
            var result = await Cli.Wrap("cmd")
                .WithArguments((args) =>
                {
                    args.Add("/c");
                    args.Add(script.Text);
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
    }
}
