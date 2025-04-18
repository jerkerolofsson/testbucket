using TestBucket.Runner.Runners;

namespace TestBucket.Runner.Poller;

public class RunObserver : IScriptRunnerObserver
{
    private readonly StringBuilder _stderr = new StringBuilder();
    private readonly StringBuilder _stdout = new StringBuilder();

    public string StdErr => _stderr.ToString();
    public string StdOut => _stdout.ToString();

    public void OnStdErr(string line)
    {
        _stderr.AppendLine(line);
    }

    public void OnStdOut(string line)
    {
        _stdout.AppendLine(line);
    }
}
