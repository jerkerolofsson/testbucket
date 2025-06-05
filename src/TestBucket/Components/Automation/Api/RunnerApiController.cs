using System.Diagnostics;

using Mediator;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TestBucket.Contracts.Runners.Models;
using TestBucket.Controllers.Api;
using TestBucket.Domain.Automation.Artifact.Events;
using TestBucket.Domain.Automation.Runners;
using TestBucket.Domain.Automation.Runners.Jobs;
using TestBucket.Domain.Automation.Runners.Registration;
using TestBucket.Domain.Shared;

namespace TestBucket.Components.Automation.Api;

[ApiController]
public class RunnerApiController : ProjectApiControllerBase
{
    private readonly IMediator _mediator;
    private readonly IJobManager _jobManager;
    private readonly JobAddedEventSignal _waiter;

    public RunnerApiController(IMediator mediator, JobAddedEventSignal waiter, IJobManager jobManager)
    {
        _mediator = mediator;
        _waiter = waiter;
        _jobManager = jobManager;
    }

    [Authorize("ApiKeyOrBearer")]
    [HttpGet("/api/runner/_health")]
    public IActionResult GetHealth()
    {
        return Ok();
    }

    [Authorize("ApiKeyOrBearer")]
    [HttpPost("/api/runner/connect")]
    public async Task ConnectRunnerAsync([FromBody] ConnectRequest request)
    {
        await _mediator.Publish(new RunnerConnectedEvent(User, request));
    }


    [Authorize("ApiKeyOrBearer")]
    [HttpPost("/api/runner/{runnerId}/jobs/{jobGuid}")]
    public async Task UpdateJobStatusAsync(string runnerId, [FromBody] RunResponse runResponse)
    {
        await _mediator.Send(new UpdateRunnerJobStatusRequest(User, runResponse));
    }


    [Authorize("ApiKeyOrBearer")]
    [HttpPost("/api/runner/{runnerId}/jobs/{jobGuid}/artifacts")]
    public async Task<IActionResult> UploadArtifactAsync([FromRoute] string runnerId, [FromRoute] string jobGuid)
    {
        var principal = User;
        var tenantId = principal.GetTenantId();
        if(tenantId is null)
        {
            return BadRequest();
        }

        var job = await _jobManager.GetJobByGuidAsync(principal, jobGuid);
        if (job?.TestRunId is null)
        {
            return BadRequest();
        }

        var pattern = "**/*.trx;**/*.xml;**/*.json;**/*.txt";
        byte[] zipBytes = await ReadRequestBodyAsByteArrayAsync();
        var request = new JobArtifactDownloaded(principal, job.TestRunId.Value, pattern, null, zipBytes);
        await _mediator.Publish(request);
        return Ok();
    }


    [EndpointDescription("Returns the next job queued for the runner")]
    [Authorize("ApiKeyOrBearer")]
    [HttpGet("/api/runner/{runnerId}/jobs")]
    public async Task<IActionResult> GetJobsAsync(string runnerId, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetJobRequest(User, runnerId));
        if(response.Request is null)
        {
            // Perform "long poll"
            await _waiter.WaitAsync(TimeSpan.FromSeconds(10));
            _waiter.Reset();
            response = await _mediator.Send(new GetJobRequest(User, runnerId));
            if(response?.Request is not null)
            {
                return Ok(response.Request);
            }
            return NoContent();
        }
        return Ok(response.Request);
    }
}
