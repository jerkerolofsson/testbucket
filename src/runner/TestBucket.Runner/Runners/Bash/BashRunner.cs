using CliWrap;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Runner.Shared;

namespace TestBucket.Runner.Runners.Bash
{
    public class BashRunner : IScriptRunner
    {
        public async Task<ScriptResult> RunAsync(Script script, IScriptRunnerObserver observer, CancellationToken cancellationToken)
        {
            if (OperatingSystem.IsWindows() || !OperatingSystem.IsLinux())
            {
                throw new NotSupportedException();
            }

            var name = Guid.NewGuid().ToString() + ".sh";
            var tempFile = Path.Combine(script.WorkingDirectory, name);

            var text = $"#!/usr/bin/bash\n{script.Text}";

            await File.WriteAllTextAsync(tempFile, text, cancellationToken);

            File.SetUnixFileMode(tempFile, UnixFileMode.UserRead | UnixFileMode.UserExecute);

            try
            {

                var result = await Cli.Wrap(tempFile)
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
