using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using Microsoft.Extensions.Logging;

using TestBucket.Contracts.Automation;
using TestBucket.Contracts.Runners.Models;
using TestBucket.Domain.ApiKeys.Validation;
using TestBucket.Domain.Automation.Runners.Models;

namespace TestBucket.Domain.Automation.Runners.Jobs
{
    public record class GetJobResponse(RunRequest? Request);
    public record class GetJobRequest(ClaimsPrincipal Principal, string RunnerId) : IRequest<GetJobResponse>;

    public class GetJobHandler : IRequestHandler<GetJobRequest, GetJobResponse>
    {
        private readonly IRunnerRepository _runnerRepository;
        private readonly IJobRepository _jobRepository;
        private readonly GetJobLock _lock;
        private readonly ILogger<GetJobHandler> _logger;

        public GetJobHandler(
            IRunnerRepository runnerRepository,
            IJobRepository jobRepository,
            GetJobLock @lock,
            ILogger<GetJobHandler> logger)
        {
            _runnerRepository = runnerRepository;
            _jobRepository = jobRepository;
            _lock = @lock;
            _logger = logger;
        }

        public async ValueTask<GetJobResponse> Handle(GetJobRequest request, CancellationToken cancellationToken)
        {
            var validator = new PrincipalValidator(request.Principal);
            var tenantId = validator.GetTenantId();
            if (tenantId is null)
            {
                return new GetJobResponse(null);
            }

            var runner = await _runnerRepository.GetByIdAsync(request.RunnerId);
            if (runner is null || runner.Languages is null || runner.Languages.Length == 0)
            {
                if (runner is not null)
                {
                    _logger.LogWarning("Runner {RunnerId} doesn't have any languages configured", request.RunnerId);
                }
                return new GetJobResponse(null);
            }

            await _lock.WaitAsync(cancellationToken);
            try
            {
                Job? job = await _jobRepository.GetOneAsync(tenantId, runner.TestProjectId, PipelineJobStatus.Queued, runner.Languages);
                if(job is not null)
                {
                    // Update the job so it won't be accepted again
                    job.Status = PipelineJobStatus.Pending;
                    await _jobRepository.UpdateAsync(job);

                    return new GetJobResponse(new RunRequest
                    {
                        TestRunId = job.TestRunId,
                        Guid = job.Guid,
                        Script = job.Script,
                        Language = job.Language,
                        EnvironmentVariables = job.EnvironmentVariables
                    });
                }

                return new GetJobResponse(null);
            }
            finally
            {
                _lock.Release();
            }
        }
    }
}
