using Microsoft.Extensions.Hosting;

namespace TestBucket.Domain.AI.Runner;
internal class AiRunner : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }
}
