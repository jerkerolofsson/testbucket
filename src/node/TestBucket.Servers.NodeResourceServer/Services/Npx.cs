namespace TestBucket.Servers.NodeResourceServer.Services;

public class Npx
{
    private static string? GetPath()
    {
        string[] npxCommands = ["npx.cmd", "npx.sh", "npx"];

        // Find the path
        var path = Environment.GetEnvironmentVariable("PATH");
        if (path is not null)
        {
            var pathItems = path.Split(';');
            foreach (var item in pathItems)
            {
                foreach (var cmd in npxCommands)
                {
                    var npxPath = System.IO.Path.Combine(item, cmd);
                    if (System.IO.File.Exists(npxPath))
                    {
                        return npxPath;
                    }
                }
            }
        }
        return null;
    }

    public static bool GetIsInstalled()
    {
        return GetPath() is not null;
    }
}
