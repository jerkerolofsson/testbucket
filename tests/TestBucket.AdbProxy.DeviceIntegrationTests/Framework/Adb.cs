using CliWrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.AdbProxy.DeviceIntegrationTests.Framework
{
    internal class Adb
    {
        private readonly string _serial;

        public Adb(string serial)
        {
            _serial = serial;
        }

        public static async Task DisconnectAllAsync()
        {
            await Cli.Wrap("adb")
              .WithArguments(["disconnect"], true)
              .WithValidation(CommandResultValidation.None)
              .ExecuteAsync(TestContext.Current.CancellationToken);
        }

        public CommandTask<CommandResult> StartLogcat(CancellationToken cancellationToken)
        {
            return Cli.Wrap("adb")
               .WithArguments(["-s", _serial, "logcat"], true)
               .WithValidation(CommandResultValidation.None)
               .ExecuteAsync(cancellationToken);
        }

        public async Task<string> GetPropAsync(string property)
        {
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken, timeout.Token);

            var sb = new StringBuilder();
            var sbError = new StringBuilder();
            try
            {
                var result = await Cli.Wrap("adb")
                   .WithArguments(["-s", _serial, "shell", "getprop", property], true)
                   .WithStandardOutputPipe(PipeTarget.ToStringBuilder(sb))
                   .WithStandardErrorPipe(PipeTarget.ToStringBuilder(sbError))
                    .ExecuteAsync(cts.Token);

                return sb.ToString();
            }
            catch(Exception ex)
            {
                throw new Exception($"stderr: {sbError}\n\nstdout: {sb}", ex);
            }
        }
    }
}
