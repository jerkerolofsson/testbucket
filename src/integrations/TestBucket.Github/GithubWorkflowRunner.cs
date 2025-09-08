using System.Diagnostics;
using System.IO.Compression;

using Microsoft.Extensions.Logging;

using Octokit;

using TestBucket.Contracts.Automation;
using TestBucket.Contracts.Integrations;
using TestBucket.Contracts.Testing.Models;
using TestBucket.Github.Models;

namespace TestBucket.Github;
internal class GithubWorkflowRunner : GithubIntegrationBaseClient, IExternalPipelineRunner
{
    public string SystemName => ExtensionConstants.SystemName;
    private readonly ILogger<GithubWorkflowRunner> _logger;

    public GithubWorkflowRunner(ILogger<GithubWorkflowRunner> logger)
    {
        _logger = logger;
    }

    public async Task CreateAsync(ExternalSystemDto system, TestExecutionContext context, CancellationToken cancellationToken)
    {
        var ownerProject = GithubOwnerProject.Parse(system.ExternalProjectId);
        string workflowFilename = context.CiCdWorkflow ?? "test.yml";

        var client = CreateClient(system);

        // Get the workflow
        var workflow = await client.Actions.Workflows.Get(ownerProject.Owner, ownerProject.Project, workflowFilename);
        if(workflow is null)
        {
            throw new ArgumentException($"The workflow {workflowFilename} could not be retreived from Github");
        }

        // We need to get the ID of the workflow run.
        // However, it is not returned from the create dispatch response.
        // There are workarounds that required adding things to the workflow, which we don't want to do.
        // Let's hope that Github adds the ID in a response in the future and use this method which
        // works "most of the time" -- classic -- if you're reading this perhaps it doesn't work now.
        // We poll for the latest workflow to change after creating a dispatch.
        var latest = await GetLatestWorkflowRunAsync(client, ownerProject);

        var createWorkflowDispatch = new CreateWorkflowDispatch(context.CiCdRef);
        createWorkflowDispatch.Inputs = new Dictionary<string, object>();
        if(context.TestSuiteName is not null)
        {
            createWorkflowDispatch.Inputs["testSuite"] = context.TestSuiteName;
        }
        /*
        foreach (var item in context.Variables.Take(10))
        {
            createWorkflowDispatch.Inputs[item.Key] = item.Value;
        }*/

        await client.Actions.Workflows.CreateDispatch(ownerProject.Owner, ownerProject.Project, workflowFilename, createWorkflowDispatch);

        var startTimestamp = Stopwatch.GetTimestamp();
        while(!cancellationToken.IsCancellationRequested)
        {
            if(Stopwatch.GetElapsedTime(startTimestamp) > TimeSpan.FromSeconds(20))
            {
                throw new TimeoutException("Failed to find workflow run ID after creating dispatch. Verify that the 'on: workflow_dispatch' is triggering the workflow for the selected ref.");
            }

            var newLatest = await GetLatestWorkflowRunAsync(client, ownerProject, run =>
            {
                return run.Path?.Contains(workflowFilename) == true;
            });
            if (newLatest is not null)
            {
                if (latest is null)
                {
                    context.CiCdPipelineIdentifier = newLatest.Id.ToString();
                    _logger.LogInformation($"Created workflow run with id: {newLatest.Id}");
                    return;
                }
                else if (latest?.Id != newLatest.Id)
                {
                    context.CiCdPipelineIdentifier = newLatest.Id.ToString();
                    _logger.LogInformation($"Created workflow run with id: {newLatest.Id}");
                    return;
                }
            }
            await Task.Delay(1000, cancellationToken);
        }
    }

    private async Task<WorkflowRun?> GetLatestWorkflowRunAsync(GitHubClient client, GithubOwnerProject ownerProject, Predicate<WorkflowRun>? predicate = null)
    {
        var now = DateTime.UtcNow;
        var runsToday = await client.Actions.Workflows.Runs.List(ownerProject.Owner, ownerProject.Project, new WorkflowRunsRequest
        {
            ExcludePullRequests = true,
            Created = $">={now.ToString("yyyy-MM-dd")}"
        });
        if (predicate is not null)
        {
            return runsToday.WorkflowRuns.Where(x=>predicate(x)).OrderByDescending(x => x.CreatedAt).FirstOrDefault();
        }
        else
        {
            return runsToday.WorkflowRuns.OrderByDescending(x => x.CreatedAt).FirstOrDefault();
        }
    }

    /// <summary>
    /// Compress all artifacts created into a single zipfile and return it
    /// </summary>
    /// <param name="system"></param>
    /// <param name="workflowRunId"></param>
    /// <param name="jobIdString"></param>
    /// <param name="testResultsArtifactsPattern"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<byte[]> GetArtifactsZipAsByteArrayAsync(ExternalSystemDto system, string workflowRunId, string jobIdString, string testResultsArtifactsPattern, CancellationToken cancellationToken)
    {
        if (!long.TryParse(jobIdString, out var jobId))
        {
            throw new ArgumentException("Expected jobIdString to be a int64");
        }
        if (!long.TryParse(workflowRunId, out var runId))
        {
            throw new ArgumentException("Expected workflowRunId to be a int64");
        }
        var ownerProject = GithubOwnerProject.Parse(system.ExternalProjectId);

        var tokenAuth = new Credentials(system.AccessToken);
        var client = new GitHubClient(new ProductHeaderValue("TestBucket"));
        client.Credentials = tokenAuth;

        PipelineDto pipeline = new();

        var artifacts  = await client.Actions.Artifacts.ListWorkflowArtifacts(ownerProject.Owner, ownerProject.Project, runId);

        // the API expects a zip file, so we download each artifact and compress them
        if(artifacts.TotalCount == 0)
        {
            return [];
        }
        using var memoryStream = new MemoryStream();
        using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create))
        {
            foreach (var artifact in artifacts.Artifacts)
            {
                _logger.LogInformation("Downloading Github artifact {ArtifactName}..", artifact.Name);
                using var sourceStream = await client.Actions.Artifacts.DownloadArtifact(ownerProject.Owner, ownerProject.Project, artifact.Id, "zip");
                using var artifactZip = new ZipArchive(sourceStream, ZipArchiveMode.Read);
                foreach (var srcEntry in artifactZip.Entries)
                {
                    using var entrySource = srcEntry.Open();
                    var newEntry = zipArchive.CreateEntry(srcEntry.FullName);
                    using var entryDest = newEntry.Open();
                    await entrySource.CopyToAsync(entryDest, cancellationToken);
                }
            }
        }
        return memoryStream.ToArray();
    }

    public  async Task<IReadOnlyList<PipelineDto>> GetPipelineRunsAsync(ExternalSystemDto system, DateOnly from, DateOnly until, CancellationToken cancellationToken)
    {
        var ownerProject = GithubOwnerProject.Parse(system.ExternalProjectId);
        var tokenAuth = new Credentials(system.AccessToken);
        var client = new GitHubClient(new ProductHeaderValue("TestBucket"));
        client.Credentials = tokenAuth;

        // Filter
        var created = $"{from.ToString("yyyy-MM-dd")}..{until.ToString("yyyy-MM-dd")}";
        var request = new WorkflowRunsRequest()
        {
            Created = created
        };
        _logger.LogInformation("Getting Github workflow runs for {GithubOwner}/{GithubProject}", ownerProject.Owner, ownerProject.Project);
        var response = await client.Actions.Workflows.Runs.List(ownerProject.Owner, ownerProject.Project, request);
        var result = new List<PipelineDto>();
        foreach (var run in response.WorkflowRuns)
        {
            result.Add(await MapRunToDtoAsync(run.Id, ownerProject, client, run));
        }
        return result;
    }

    public async Task<PipelineDto?> GetPipelineAsync(ExternalSystemDto system, string workflowRunId, CancellationToken cancellationToken)
    {
        if (!long.TryParse(workflowRunId, out var runId))
        {
            throw new ArgumentException("Expected workflowRunId to be a int64");
        }

        var ownerProject = GithubOwnerProject.Parse(system.ExternalProjectId);

        var tokenAuth = new Credentials(system.AccessToken);
        var client = new GitHubClient(new ProductHeaderValue("TestBucket"));
        client.Credentials = tokenAuth;

        var run = await client.Actions.Workflows.Runs.Get(ownerProject.Owner, ownerProject.Project, runId);
        return await MapRunToDtoAsync(runId, ownerProject, client, run);
    }

    private static async Task<PipelineDto> MapRunToDtoAsync(long runId, GithubOwnerProject ownerProject, GitHubClient client, WorkflowRun run)
    {
        PipelineDto pipeline = new();
        pipeline.CiCdPipelineIdentifier = run.Id.ToString();
        pipeline.CiCdProjectId = ownerProject.ToString();
        pipeline.DisplayTitle = run.DisplayTitle;
        pipeline.HeadCommit = run.HeadCommit?.Sha;

        if (run.Status.TryParse(out var status))
        {
            switch (status)
            {
                case WorkflowRunStatus.Completed:
                    pipeline.Status = PipelineStatus.Completed;
                    break;
                case WorkflowRunStatus.InProgress:
                    pipeline.Status = PipelineStatus.Running;
                    break;
                case WorkflowRunStatus.Queued:
                    pipeline.Status = PipelineStatus.Queued;
                    break;
                case WorkflowRunStatus.Waiting:
                    pipeline.Status = PipelineStatus.Waiting;
                    break;
                case WorkflowRunStatus.Pending:
                    pipeline.Status = PipelineStatus.Pending;
                    break;
            }
        }

        pipeline.WebUrl = run.Url;
        pipeline.Duration = run.UpdatedAt - run.CreatedAt;

        var jobResponse = await client.Actions.Workflows.Jobs.List(ownerProject.Owner, ownerProject.Project, runId);
        foreach (var job in jobResponse.Jobs)
        {
            var jobDto = new PipelineJobDto()
            {
                CiCdJobIdentifier = job.Id.ToString(),
                CreatedAt = job.CreatedAt,
                FinishedAt = job.CompletedAt,
                StartedAt = job.StartedAt,
                WebUrl = job.Url,
                Name = job.Name
            };
            pipeline.Jobs.Add(jobDto);

            if (jobDto.FinishedAt is not null && jobDto.StartedAt is not null)
            {
                jobDto.Duration = jobDto.FinishedAt - jobDto.StartedAt;
            }
            else if (jobDto.StartedAt is not null)
            {
                jobDto.Duration = DateTimeOffset.UtcNow - jobDto.StartedAt.Value;
            }

            if (job.Status.TryParse(out var jobStatus))
            {
                switch (status)
                {
                    case WorkflowRunStatus.Completed:
                        jobDto.Status = PipelineJobStatus.Completed;
                        jobDto.HasArtifacts = true;
                        break;
                    case WorkflowRunStatus.InProgress:
                        jobDto.Status = PipelineJobStatus.Running;
                        break;
                    case WorkflowRunStatus.Queued:
                        jobDto.Status = PipelineJobStatus.Queued;
                        break;
                    case WorkflowRunStatus.Waiting:
                        jobDto.Status = PipelineJobStatus.Waiting;
                        break;
                    case WorkflowRunStatus.Pending:
                        jobDto.Status = PipelineJobStatus.Pending;
                        break;
                }
            }
        }

        return pipeline;
    }

    public Task<string> ReadTraceAsync(ExternalSystemDto system, string jobId, CancellationToken cancellationToken)
    {
        var trace = "";
        return Task.FromResult(trace);
    }
}
