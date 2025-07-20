using System.Threading.Channels;

using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.AI.Runner;
internal class AiRunnerJobQueue
{
    private readonly Channel<AiRunnerJob> _channel = Channel.CreateBounded<AiRunnerJob>(100);

    public async Task EnqueueAsync(TestCaseRun run)
    {
        await _channel.Writer.WriteAsync(new AiRunnerJob(run));
    }

    public async Task <AiRunnerJob?> DequeueAsync(CancellationToken cancellationToken)
    {
        if (await _channel.Reader.WaitToReadAsync(cancellationToken))
        {
            return await _channel.Reader.ReadAsync(cancellationToken);
        }
        return null;
    }
}
