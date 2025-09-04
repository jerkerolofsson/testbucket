using CliWrap;

using TestBucket.Runner.Shared;

namespace TestBucket.Runner.Runners.Powershell
{
    public class PythonRunner : IScriptRunner
    {
        private static string? _exe;
        private static readonly string[] _filenamesLinux = ["python3", "python", "py"];
        private static readonly string[] _filenamesWindows = ["python3.exe", "python.exe", "py.exe"];

        public async Task<ScriptResult> RunAsync(Script script, IScriptRunnerObserver observer, CancellationToken cancellationToken)
        {
            var name = Guid.NewGuid().ToString() + ".py";
            var tempFile = Path.Combine(script.WorkingDirectory, name);
            await File.WriteAllTextAsync(tempFile, script.Text, cancellationToken);

            try
            {
                string exe = ResolveExecutable();

                var result = await Cli.Wrap(exe)
                    .WithArguments((args) =>
                    {
                        args.Add(tempFile);
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

        internal static string ResolveExecutable()
        {
            if (_exe != null)
            {
                return _exe;
            }
            else
            {
                var candidates = OperatingSystem.IsWindows() ? _filenamesWindows : _filenamesLinux;
                var pathEnv = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
                var pathDirs = pathEnv.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);

                foreach (var candidate in candidates)
                {
                    // If candidate is already an absolute/relative path, check it directly.
                    if (Path.IsPathRooted(candidate))
                    {
                        if (File.Exists(candidate))
                        {
                            _exe = candidate;
                            return _exe;
                        }

                        continue;
                    }

                    // Search each PATH directory for the candidate.
                    foreach (var dir in pathDirs)
                    {
                        try
                        {
                            var full = Path.Combine(dir, candidate);
                            if (File.Exists(full))
                            {
                                _exe = full;
                                return _exe;
                            }
                        }
                        catch
                        {
                            // ignore problematic PATH entries
                        }
                    }
                }
            }

            // No candidate found on PATH — fall back to a sensible default.
            _exe = OperatingSystem.IsWindows() ? "python" : "python3";
            return _exe;
        }
    }
}
