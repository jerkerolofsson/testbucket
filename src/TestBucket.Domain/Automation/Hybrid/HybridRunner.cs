using System.Security.Claims;

using TestBucket.Contracts.Automation;
using TestBucket.Contracts.Integrations;
using TestBucket.Domain.ApiKeys.Validation;
using TestBucket.Domain.Automation.Runners;
using TestBucket.Domain.Automation.Runners.Jobs;
using TestBucket.Domain.Automation.Runners.Models;

namespace TestBucket.Domain.Automation.Hybrid
{
    public class HybridRunner : IMarkdownTestRunner
    {
        private readonly IRunnerRepository _repository;
        private readonly IJobManager _jobManager;
        private readonly JobAddedEventSignal _jobAddedEventSignal;

        public HybridRunner(IRunnerRepository repository, IJobManager jobManager, JobAddedEventSignal jobAddedEventSignal)
        {
            _repository = repository;
            _jobManager = jobManager;
            _jobAddedEventSignal = jobAddedEventSignal;
        }

        /// <summary>
        /// Returns supported runs
        /// </summary>
        /// <param name="context"></param>
        /// <param name="code"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<TestRunnerResult> RunAsync(ClaimsPrincipal principal, TestExecutionContext context, string language, string code, CancellationToken cancellationToken)
        {
            Job job = CreateJob(context, language, code);
            job.TenantId = principal.GetTenantIdOrThrow();
            await _jobManager.AddAsync(principal, job);

            // Notify long-polling runners
            _jobAddedEventSignal.Set(job.Guid);

            return await WaitForJobCompletionAsync(principal, job, cancellationToken);
        }

        private async Task<TestRunnerResult> WaitForJobCompletionAsync(ClaimsPrincipal principal, Job job, CancellationToken cancellationToken)
        {
            int sleepTime = 100;
            int loopCounter = 0;
            while (true)
            {
                var updatedJob = await _jobManager.GetByIdAsync(principal, job.Id);
                if (updatedJob is null)
                {
                    return new TestRunnerResult { Completed = false, ErrorMessage = "Job deleted", Format = Formats.TestResultFormat.UnknownFormat, Result = "", Success = false };
                }
                switch (updatedJob.Status)
                {
                    case PipelineJobStatus.Success:
                    case PipelineJobStatus.Canceled:
                    case PipelineJobStatus.Failed:
                    case PipelineJobStatus.Completed:
                    case PipelineJobStatus.Skipped:
                        // The job is completed, and attachments should have been uploaded
                        return new TestRunnerResult 
                        { 
                            Success = updatedJob.Status == PipelineJobStatus.Success,
                            StdOut = updatedJob.StdOut??"",
                            StdErr = updatedJob.StdErr ?? "",
                            Completed = true, 
                            Format = updatedJob.Format ?? Formats.TestResultFormat.UnknownFormat,  
                            Result = updatedJob.Result ?? "", 
                            ArtifactContent = updatedJob.ArtifactContent,
                        };
                    case PipelineJobStatus.Error:
                        return new TestRunnerResult { Completed = true, Format = Formats.TestResultFormat.UnknownFormat, Result = "", ErrorMessage = updatedJob.ErrorMessage, Success = false};
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    return new TestRunnerResult { Completed = false, ErrorMessage = "Timeout", Format = Formats.TestResultFormat.UnknownFormat, Result = "", Success = false };
                }
                await Task.Delay(TimeSpan.FromMilliseconds(sleepTime), cancellationToken);

                // Back off
                loopCounter++;
                if (loopCounter > 10)
                {
                    sleepTime = 500;
                }
                if (loopCounter > 20)
                {
                    sleepTime = 1000;
                }
                if (loopCounter > 30)
                {
                    sleepTime = 2000;
                }
            }
        }

        internal static Job CreateJob(TestExecutionContext context, string language, string code)
        {
            return new Job
            {
                Guid = Guid.NewGuid().ToString(),
                TestRunId = context.TestRunId,
                TestProjectId = context.ProjectId,
                Language = language,
                Script = code,
                Priority = 1,
                Status = PipelineJobStatus.Queued,
                EnvironmentVariables = context.Variables
            };
        }

        public async Task<bool> SupportsLanguageAsync(ClaimsPrincipal principal, string language)
        {
            var languages = await GetSupportedLanguagesAsync(principal);
            return languages.Contains(language);
        }


        /// <summary>
        /// Returns supported languages
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public async Task<string[]> GetSupportedLanguagesAsync(ClaimsPrincipal principal)
        {
            var supportedLanguages = new HashSet<string>();
            var tenantId = principal.GetTenantIdOrThrow();
            var validator = new PrincipalValidator(principal);
            var projectId = validator.GetProjectId();
            foreach (var runner in await _repository.GetAllAsync(tenantId))
            {
                if (runner.Languages is not null && (runner.TestProjectId is null || runner.TestProjectId == projectId))
                {
                    foreach (var language in runner.Languages)
                    {
                        if (!supportedLanguages.Contains(language))
                        {
                            supportedLanguages.Add(language);
                        }
                    }
                }
            }
            return supportedLanguages.ToArray();
        }
    }
}
