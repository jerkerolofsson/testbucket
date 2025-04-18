using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Runner.Runners
{
    public interface IScriptRunner
    {
        Task<ScriptResult> RunAsync(Script script, IScriptRunnerObserver observer, CancellationToken cancellationToken);
    }
}
