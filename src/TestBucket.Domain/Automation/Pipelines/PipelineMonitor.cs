using TestBucket.Contracts.Automation;
using TestBucket.Domain.Automation.Pipelines.Models;

namespace TestBucket.Domain.Automation.Pipelines;

/// <summary>
/// Monitors a pipeline, updating the state locally
/// </summary>
internal class PipelineMonitor
{
    private readonly PipelineManager _pipelineManager;
    private readonly ClaimsPrincipal _principal;
    private readonly Pipeline _pipeline;

    public PipelineMonitor(PipelineManager pipelineManager, ClaimsPrincipal principal, Pipeline pipeline)
    {
        _pipelineManager = pipelineManager;
        _principal = principal;
        _pipeline = pipeline;
    }

    private static bool IsCompleted(Pipeline pipeline)
    {
        return pipeline.Status is PipelineStatus.Failed or PipelineStatus.Completed or PipelineStatus.Success or PipelineStatus.Error;
    }

    internal void Start()
    {
        if(string.IsNullOrWhiteSpace(_pipeline.CiCdPipelineIdentifier))
        {
            return;
        }

        var _ = Task.Run(async () =>
        {
            DateTimeOffset lastSuccess = DateTimeOffset.UtcNow;
            while (true)
            {
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                try
                {
                    var pipeline = await _pipelineManager.RefreshStatusAsync(_principal, _pipeline.Id, cts.Token);
                    if (pipeline is not null)
                    {
                        lastSuccess = DateTimeOffset.UtcNow;
                        if (IsCompleted(pipeline))
                        {
                            // Pipeline has completed
                            await _pipelineManager.OnPipelineCompletedAsync(_principal, pipeline);
                            return;
                        }
                    }
                }
                catch { }

                // Give up if not accessible
                var elapsedSinceSuccess = DateTimeOffset.UtcNow - lastSuccess;
                if(elapsedSinceSuccess > TimeSpan.FromMinutes(120))
                {
                    return;
                }

                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        });

    }
}
