using NGitLab;
using NGitLab.Models;

using TestBucket.Contracts.Automation;
using TestBucket.Contracts.Integrations;
using TestBucket.Contracts.Testing.Models;

namespace TestBucket.Gitlab
{
    public class GitlabPipelineRunner : IExternalPipelineRunner
    {
        private readonly HttpClient _httpClient;

        public GitlabPipelineRunner(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public string SystemName => ExtensionConstants.SystemName;

        /// <summary>
        /// Downloads artifacts from the job as a zip
        /// </summary>
        /// <param name="system"></param>
        /// <param name="jobId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<byte[]> GetArtifactsZipAsByteArrayAsync(ExternalSystemDto system, string pipelineId, string jobId, string testResultsArtifactsPattern, CancellationToken cancellationToken)
        {
            if (system is not null &&
               system.ExternalProjectId is not null &&
               system.BaseUrl is not null &&
               system.AccessToken is not null &&
               long.TryParse(system.ExternalProjectId, out long projectId) &&
               long.TryParse(jobId, out long id))
            {
                var client = new GitLabClient(system.BaseUrl, system.AccessToken);
                var jobsClient = client.GetJobs(projectId);
                return await Task.Run(() => jobsClient.GetJobArtifacts(id));
            }
            throw new ArgumentException("GetArtifactsAsync requires a valid project id and job id");
        }

        /// <summary>
        /// Reads the log file from a job
        /// </summary>
        /// <param name="system"></param>
        /// <param name="jobId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<string> ReadTraceAsync(ExternalSystemDto system, string jobId, CancellationToken cancellationToken)
        {
            if (system is not null &&
               system.ExternalProjectId is not null &&
               system.BaseUrl is not null &&
               system.AccessToken is not null &&
               long.TryParse(system.ExternalProjectId, out long projectId) &&
               long.TryParse(jobId, out long id))
            {
                var client = new GitLabClient(system.BaseUrl, system.AccessToken);
                var jobsClient = client.GetJobs(projectId);

                return await jobsClient.GetTraceAsync(id, cancellationToken);
            }
            throw new ArgumentException("ReadTraceAsync requires a valid project id and job id");
        }

        /// <summary>
        /// Reads the latest status for a pipeline, including jobs, and returns it
        /// </summary>
        /// <param name="system"></param>
        /// <param name="pipelineId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<PipelineDto?> GetPipelineAsync(ExternalSystemDto system, string pipelineId, CancellationToken cancellationToken)
        {
            if (system is not null &&
                system.ExternalProjectId is not null &&
                system.BaseUrl is not null &&
                system.AccessToken is not null &&
                long.TryParse(system.ExternalProjectId, out long projectId) &&
                long.TryParse(pipelineId, out long id))
            {
                var client = new GitLabClient(system.BaseUrl, system.AccessToken);
                var pipelineClient = client.GetPipelines(projectId);
                var pipeline = await pipelineClient.GetByIdAsync(id, cancellationToken);

                return await MapPipelineAsync(system, pipelineClient, pipeline);
            }
            return null;
        }

        private async Task<PipelineDto> MapPipelineAsync(ExternalSystemDto system, IPipelineClient pipelineClient, Pipeline pipeline)
        {
            TimeSpan? duration = null;
            if (pipeline.Duration is not null)
            {
                duration = TimeSpan.FromSeconds(pipeline.Duration.Value);
            }

            var jobsResponse = pipelineClient.GetJobsAsync(new PipelineJobQuery { PipelineId = pipeline.Id });
            List<Job> jobs = [];
            await foreach (var job in jobsResponse)
            {
                jobs.Add(job);
            }

            return new PipelineDto
            {
                CiCdPipelineIdentifier = pipeline.Id.ToString(),
                CiCdProjectId = system.ExternalProjectId,
                WebUrl = pipeline.WebUrl,
                Error = pipeline.YamlError,
                Duration = duration,
                Jobs = MapJobs(jobs),
                Status = pipeline.Status switch
                {
                    JobStatus.Running => PipelineStatus.Running,
                    JobStatus.Canceled => PipelineStatus.Canceled,
                    JobStatus.Pending => PipelineStatus.Pending,
                    JobStatus.Canceling => PipelineStatus.Canceling,
                    JobStatus.Preparing => PipelineStatus.Preparing,
                    JobStatus.Created => PipelineStatus.Created,
                    JobStatus.Failed => PipelineStatus.Failed,
                    JobStatus.NoBuild => PipelineStatus.NoBuild,
                    JobStatus.Success => PipelineStatus.Success,
                    JobStatus.Skipped => PipelineStatus.Skipped,
                    JobStatus.WaitingForResource => PipelineStatus.WaitingForResource,
                    JobStatus.Scheduled => PipelineStatus.Scheduled,
                    JobStatus.Manual => PipelineStatus.Manual,
                    _ => PipelineStatus.Unknown,
                }
            };
        }

        private List<PipelineJobDto> MapJobs(IEnumerable<Job> jobs)
        {
            var dtos = new List<PipelineJobDto>();

            foreach (var job in jobs)
            {
                bool hasArtifacts = job.Artifacts?.Size > 0;

                if (job.Status == JobStatus.Success)
                {

                }

                var dto = new PipelineJobDto
                {
                    CiCdJobIdentifier = job.Id.ToString(),
                    AllowFailure = job.AllowFailure,
                    Name = job.Name,
                    Coverage = job.Coverage,
                    CreatedAt = job.CreatedAt.ToUniversalTime(),
                    FinishedAt = job.FinishedAt.ToUniversalTime(),
                    Stage = job.Stage,
                    FailureReason = job.FailureReason,
                    StartedAt = job.StartedAt.ToUniversalTime(),
                    TagList = job.TagList,
                    WebUrl = job.WebUrl,
                    HasArtifacts = hasArtifacts
                };

                if (job.Duration is not null)
                {
                    dto.Duration = TimeSpan.FromSeconds(job.Duration.Value);
                }
                if (job.QueuedDuration is not null)
                {
                    dto.QueuedDuration = TimeSpan.FromSeconds(job.QueuedDuration.Value);
                }

                dto.Status = job.Status switch
                {
                    JobStatus.Running => PipelineJobStatus.Running,
                    JobStatus.Canceled => PipelineJobStatus.Canceled,
                    JobStatus.Pending => PipelineJobStatus.Pending,
                    JobStatus.Canceling => PipelineJobStatus.Canceling,
                    JobStatus.Preparing => PipelineJobStatus.Preparing,
                    JobStatus.Created => PipelineJobStatus.Created,
                    JobStatus.Failed => PipelineJobStatus.Failed,
                    JobStatus.NoBuild => PipelineJobStatus.NoBuild,
                    JobStatus.Success => PipelineJobStatus.Success,
                    JobStatus.Skipped => PipelineJobStatus.Skipped,
                    JobStatus.WaitingForResource => PipelineJobStatus.WaitingForResource,
                    JobStatus.Scheduled => PipelineJobStatus.Scheduled,
                    JobStatus.Manual => PipelineJobStatus.Manual,
                    _ => PipelineJobStatus.Unknown,
                };
                dtos.Add(dto);
            }

            return dtos;
        }

        /// <summary>
        /// Creates a pipeline and, if successful, sets the pipeline identifier (this is unique to the external system used) as
        /// the context.CiCdPipelineIdentifier property
        /// </summary>
        /// <param name="system"></param>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task CreateAsync(ExternalSystemDto system, TestExecutionContext context, CancellationToken cancellationToken)
        {
            if (system is not null &&
                system.ExternalProjectId is not null &&
                system.BaseUrl is not null &&
                system.AccessToken is not null &&
                long.TryParse(system.ExternalProjectId, out long projectId))
            {
                var client = new GitLabClient(system.BaseUrl, system.AccessToken);
                var pipelineClient = client.GetPipelines(projectId);

                var options = new PipelineCreate
                {
                    Ref = context.CiCdRef,
                };
                foreach (var kvp in context.Variables)
                {
                    options.Variables[kvp.Key] = kvp.Value;
                }
                var pipeline = await pipelineClient.CreateAsync(options, cancellationToken);
                context.CiCdPipelineIdentifier = pipeline.Id.ToString();
            }
        }

        public async Task<IReadOnlyList<PipelineDto>> GetPipelineRunsAsync(ExternalSystemDto system, DateOnly from, DateOnly until, CancellationToken cancellationToken)
        {
            var result = new List<PipelineDto>();

            if (system is not null &&
                system.ExternalProjectId is not null &&
                system.BaseUrl is not null &&
                system.AccessToken is not null &&
                long.TryParse(system.ExternalProjectId, out long projectId))
            {
                var client = new GitLabClient(system.BaseUrl, system.AccessToken);
                var pipelineClient = client.GetPipelines(projectId);

                var query = new PipelineQuery
                {
                    UpdatedBefore = until.ToDateTime(new TimeOnly(0, 0, 0)),
                    UpdatedAfter = from.ToDateTime(new TimeOnly(23, 59, 59))
                };
                var response = pipelineClient.Search(query);
                foreach(var item in response)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var pipeline = await pipelineClient.GetByIdAsync(item.Id, cancellationToken);
                    result.Add(await MapPipelineAsync(system, pipelineClient, pipeline));
                }
            }

            return result;
        }
    }
}
