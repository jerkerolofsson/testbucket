using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Runner.Runners
{
    public interface IScriptRunnerObserver
    {
        void OnStdOut(string line);
        void OnStdErr(string line);
    }
}
