using CliWrap;

namespace TestBucket.Servers.NodeResourceServer.Services.Playwright;

public class PlaywrightService
{
    public static Task RunAsync(int port, ILogger logger, CancellationToken cancellationToken)
    {
        string[] args = ["-y", "@playwright/mcp@latest", "--isolated", "--port", port.ToString()];

        var task = Task.Run(async () =>
        {
            var npx = Cli.Wrap("npx")
                .WithArguments(args)
                .WithStandardErrorPipe(PipeTarget.ToDelegate(x => logger.LogInformation(x)))
                .WithStandardOutputPipe(PipeTarget.ToDelegate(x => logger.LogInformation(x)))
                .ExecuteAsync(cancellationToken);

            logger.LogInformation("npx playwright/mcp with port={port} and pid={NpxPlaywrightPid} started", port, npx.ProcessId);

            await npx;

            logger.LogWarning("npx playwright/mcp with port={port} and pid={NpxPlaywrightPid} exited", port, npx.ProcessId);

        });

        

        return task;
    }
}
