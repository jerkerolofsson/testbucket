using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Automation.Models;

namespace TestBucket.Domain.Automation.Services;
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
                        if (pipeline.IsCompleted)
                        {
                            // Pipeline has completed
                            return;
                        }
                    }
                }
                catch { }

                var elapsedSinceSucces = DateTimeOffset.UtcNow - lastSuccess;
                if(elapsedSinceSucces > TimeSpan.FromMinutes(10))
                {
                    return;
                }

                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        });

    }
}
