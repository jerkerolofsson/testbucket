using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Contracts.Runners.Models;

namespace TestBucket.Domain.Automation.Runners.Jobs
{
    public record class UpdateRunnerJobStatusResponse(bool Success);
    public record class UpdateRunnerJobStatusRequest(ClaimsPrincipal Principal, RunResponse Response) : IRequest<UpdateRunnerJobStatusResponse>;

    /// <summary>
    /// Updates the job status in the database
    /// This is invoked through an API, for example by the "Test Bucket Runner"
    /// </summary>
    public class UpdateRunnerJobStatusHandler : IRequestHandler<UpdateRunnerJobStatusRequest, UpdateRunnerJobStatusResponse>
    {
        private readonly IJobRepository _jobRepository;

        public UpdateRunnerJobStatusHandler(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }

        public async ValueTask<UpdateRunnerJobStatusResponse> Handle(UpdateRunnerJobStatusRequest request, CancellationToken cancellationToken)
        {
            var job = await _jobRepository.GetByGuidAsync(request.Response.Guid);
            if(job is null)
            {
                return new UpdateRunnerJobStatusResponse(Success: false);
            }

            var principal = request.Principal;
            principal.ThrowIfEntityTenantIsDifferent(job);

            job.StdOut = request.Response.StdOut;
            job.StdErr = request.Response.StdErr;
            job.Status = request.Response.Status;
            job.ErrorMessage = request.Response.ErrorMessage;
            job.Result = request.Response.Result;
            job.Format = request.Response.Format;
            job.ArtifactContent = request.Response.ArtifactContent;

            //job.TestResultFormat = request.Response.TestResultFormat;
            //job.ResultContent = request.Response.ResultContent;

            await _jobRepository.UpdateAsync(job);

            return new UpdateRunnerJobStatusResponse(Success: true);
        }
    }
}
