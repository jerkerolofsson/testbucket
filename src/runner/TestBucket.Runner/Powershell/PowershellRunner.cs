using CliWrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Runner.Shared;

namespace TestBucket.Runner.Powershell
{
    public class PowershellRunner : IScriptRunner
    {
        public async Task<ScriptResult> RunAsync(Script script, IScriptRunnerObserver observer, CancellationToken cancellationToken)
        {
            var name = Guid.NewGuid().ToString() + ".ps1";
            var tempFile = Path.Combine(script.WorkingDirectory, name);
            await File.WriteAllTextAsync(tempFile, script.Text, cancellationToken);

            try
            {

                var result = await Cli.Wrap("powershell")
                    .WithArguments((args) =>
                    {
                        args.Add("/f");
                        args.Add(tempFile);
                        //args.Add(script.Text);
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
