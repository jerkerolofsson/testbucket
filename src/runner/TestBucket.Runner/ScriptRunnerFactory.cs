using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Runner.Cmd;

namespace TestBucket.Runner
{
    public class ScriptRunnerFactory
    {
        public static IScriptRunner Create(string name)
        {
            return name switch
            {
                "cmd" => new CmdRunner(),
                "pwsh" => new Powershell.PowershellRunner(),
                "powershell" => new Powershell.PowershellRunner(),
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
