
using System.Reflection.Metadata;

using DotNet.Globbing;

using TestBucket.Contracts.Runners.Models;

namespace TestBucket.Runner.Workspaces;

public class TestWorkspace : IDisposable
{
    private readonly string _directoryPath;

    public string WorkingDirectory => _directoryPath;

    public TestWorkspace()
    {
        string directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
            "test-bucket", "runner", "workspaces", Guid.NewGuid().ToString());
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        _directoryPath = directoryPath;
    }

    public void Dispose()
    {
        Directory.Delete(_directoryPath, true);
    }


    internal IEnumerable<FileInfo> GetArtifacts(string globPatternsString)
    {
        var globPatterns = globPatternsString.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var globs = globPatterns.Select(globPatterns => Glob.Parse(globPatterns));

        var directoryInfo = new DirectoryInfo(_directoryPath);
        foreach(var file in directoryInfo.GetFiles("*", SearchOption.AllDirectories))
        {
            foreach (var glob in globs)
            {
                if (glob.IsMatch(file.FullName))
                {
                    yield return file;
                    break;
                }
            }
        }
    }
    internal IEnumerable<FileInfo> GetArtifacts()
    {
        var directoryInfo = new DirectoryInfo(_directoryPath);
        return directoryInfo.GetFiles("*", SearchOption.AllDirectories);
    }
}
