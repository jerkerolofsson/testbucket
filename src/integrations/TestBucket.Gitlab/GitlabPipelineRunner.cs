using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using NGitLab;
using NGitLab.Models;

using TestBucket.Contracts.Integrations;
using TestBucket.Contracts.Projects;
using TestBucket.Contracts.Testing.Models;
using TestBucket.Formats.Dtos;

namespace TestBucket.Gitlab
{
    public class GitlabPipelineRunner : IExternalPipelineRunner
    {
        public string SystemName => "GitLab";

        public async Task StartAsync(ExternalSystemDto system, TestExecutionContext context, CancellationToken cancellationToken)
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
