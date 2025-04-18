using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Runner.Runners;

namespace TestBucket.Runner
{
    class ConsoleObserver : IScriptRunnerObserver
    {
        public void OnStdErr(string line)
        {
            Console.WriteLine(line);
        }

        public void OnStdOut(string line)
        {
            Console.WriteLine(line);
        }
    }
}
