using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using NGitLab;
using NGitLab.Models;

using TestBucket.Contracts.Automation;
using TestBucket.Contracts.Integrations;
using TestBucket.Contracts.Projects;
using TestBucket.Contracts.Testing.Models;
using TestBucket.Formats.Dtos;

namespace TestBucket.Gitlab
{
    public class GitlabPipelineRunner : IExternalPipelineRunner
    {
        public string SystemName => "GitLab";

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

                TimeSpan? duration = null;
                if (pipeline.Duration is not null)
                {
                    duration = TimeSpan.FromSeconds(pipeline.Duration.Value);
                }
                return new PipelineDto
                {
                    WebUrl = pipeline.WebUrl,
                    Error = pipeline.YamlError,
                    Duration = duration,
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
            return null;
        }

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
                foreach(var kvp in context.Variables)
                {
                    options.Variables[kvp.Key] = kvp.Value;
                }
                var pipeline = await pipelineClient.CreateAsync(options, cancellationToken);
                context.CiCdPipelineIdentifier = pipeline.Id.ToString();

                
            }
        }
    }
}
