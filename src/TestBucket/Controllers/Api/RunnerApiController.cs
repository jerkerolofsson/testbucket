using System.Diagnostics;

using Mediator;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TestBucket.Contracts.Runners.Models;
using TestBucket.Domain.Automation.Runners.Jobs;
using TestBucket.Domain.Automation.Runners.Registration;

namespace TestBucket.Controllers.Api;

[ApiController]
public class RunnerApiController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly JobAddedEventSignal _waiter;

    public RunnerApiController(IMediator mediator, JobAddedEventSignal waiter)
    {
        _mediator = mediator;
        _waiter = waiter;
    }

    [Authorize("ApiKeyOrBearer")]
    [HttpGet("/api/runner/_health")]
    public IActionResult GetHealth()
    {
        return Ok();
    }

    [Authorize("ApiKeyOrBearer")]
    [HttpPost("/api/runner/connect")]
    public async Task ConnectRunner([FromBody] ConnectRequest request)
    {
        await _mediator.Publish(new RunnerConnectedEvent(this.User, request));
    }


    [Authorize("ApiKeyOrBearer")]
    [HttpPost("/api/runner/{runnerId}/jobs/{jobGuid}")]
    public async Task UpdateJobStatusd(string runnerId, [FromBody] RunResponse runResponse)
    {
        await _mediator.Send(new UpdateRunnerJobStatusRequest(this.User, runResponse));
    }

    [EndpointDescription("Returns the next job queued for the runner")]
    [Authorize("ApiKeyOrBearer")]
    [HttpGet("/api/runner/{runnerId}/jobs")]
    public async Task<IActionResult> GetJobs(string runnerId, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetJobRequest(this.User, runnerId));
        if(response.Request is null)
        {
            // Perform "long poll"
            var startTimestamp = Stopwatch.GetTimestamp();
            while(!cancellationToken.IsCancellationRequested)
            {
                await _waiter.WaitAsync(TimeSpan.FromSeconds(10));
                response = await _mediator.Send(new GetJobRequest(this.User, runnerId));
                if(response is not null)
                {
                    return Ok(response.Request);
                }

                // Poll for 30 seconds max
                if(Stopwatch.GetElapsedTime(startTimestamp) > TimeSpan.FromSeconds(30))
                {
                    break;
                }
            }

            return NoContent();
        }
        return Ok(response.Request);
    }
}
