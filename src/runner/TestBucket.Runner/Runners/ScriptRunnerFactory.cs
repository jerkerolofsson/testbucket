using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Runner.Runners.Cmd;
using TestBucket.Runner.Runners.Http;
using TestBucket.Runner.Runners.Powershell;

namespace TestBucket.Runner.Runners
{
    public class ScriptRunnerFactory
    {
        public static IScriptRunner Create(string name)
        {
            return name switch
            {
                "cmd" => new CmdRunner(),
                "pwsh" => new PowershellRunner(),
                "powershell" => new PowershellRunner(),
                "http" => new DotHttpRunner(),
                _ => throw new ArgumentException($"Invalid runner: {name}")
            };
        }
        public static async Task<ScriptResult> RunAsync(Script script, IScriptRunnerObserver observer, CancellationToken cancellationToken)
        {
            try
            {
                var runner = Create(script.RunnerType);
                return await runner.RunAsync(script, observer, cancellationToken);
            }
            catch(Exception ex)
            {
                return new ScriptResult
                {
                    Success = false,
                    Exception = ex
                };
            }
        }
    }
}
