﻿using CliWrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Runner.Shared
{
    public static class CliWrapExtensions
    {
        public static CliWrap.Command Enrich(this CliWrap.Command cmd, Script script)
        {
            return cmd.WithEnvironmentVariables((env) =>
            {
                if (script.EnvironmentVariables is not null)
                {
                    env.Set(script.EnvironmentVariables);
                }
            });
        }
        public static CliWrap.Command LinkObserver(this CliWrap.Command cmd, IScriptRunnerObserver observer)
        {
            return 
                cmd.WithStandardOutputPipe(PipeTarget.ToDelegate(observer.OnStdOut))
                .WithStandardErrorPipe(PipeTarget.ToDelegate(observer.OnStdErr));
        }
    }
}
