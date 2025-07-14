using CliWrap;

namespace TestBucket.Servers.NodeResourceServer.Services.Playwright;

public class PlaywrightService
{
    public static Task RunAsync(int port, CancellationToken cancellationToken)
    {
        string[] args = ["-y", "@playwright/mcp@latest", "--isolated", "--port", port.ToString()];

        return Task.Run(async () =>
        {
            await Cli.Wrap("npx")
                .WithArguments(args)
                .ExecuteAsync(cancellationToken);
        });
    }
}
