using System.IO.Compression;

using TestBucket.Contracts.Automation;
using TestBucket.Contracts.Runners.Models;
using TestBucket.Runner.Runners;
using TestBucket.Runner.Settings;
using TestBucket.Runner.Workspaces;

namespace TestBucket.Runner.Poller;

public class GetJobPoller(
    ILogger<GetJobPoller> Logger,
    TestBucketApiClient Client, 
    SettingsManager SettingsManager) : BackgroundService
{

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(5000, stoppingToken);
        var settings = await SettingsManager.LoadSettingsAsync();
        while(!stoppingToken.IsCancellationRequested && settings.AccessToken is not null && settings.Id is not null)
        {
            try
            {
                var job = await Client.GetJobAsync(settings.Id, settings.AccessToken);
                if(job is not null)
                {
                    if (job.Language is null)
                    {
                        var response = new RunResponse { Guid = job.Guid, Accepted = false, ErrorMessage = "No language is specified", Status = PipelineJobStatus.Error };
                        await Client.PostJobResponseAsync(settings.Id, response, settings.AccessToken);
                    }
                    else
                    {
                        var response = new RunResponse { Guid = job.Guid, Accepted = true, Status = PipelineJobStatus.Waiting };
                        await Client.PostJobResponseAsync(settings.Id, response, settings.AccessToken);
                        await RunJobAsync(settings.Id, job, settings.AccessToken, stoppingToken);
                    }
                }
            }
            catch(HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                // Poll again
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to poll for jobs");
            }
        }
    }

    private async Task RunJobAsync(string runnerId, RunRequest job, string accessToken, CancellationToken cancellationToken)
    {
        try
        {
            using var workspace = new TestWorkspace();
            var observer = new RunObserver();

            var script = new Script
            {
                RunnerType = job.Language ?? throw new Exception("Language not set"),
                EnvironmentVariables = job.EnvironmentVariables,
                Text = job.Script ?? throw new Exception("Script not set"),
                WorkingDirectory = workspace.WorkingDirectory
            };

            // Post running state
            var response = new RunResponse { Guid = job.Guid, Accepted = true, Status = PipelineJobStatus.Running };
            await Client.PostJobResponseAsync(runnerId, response, accessToken);

            var scriptRunner = ScriptRunnerFactory.Create(script.RunnerType);
            var scriptResult = await scriptRunner.RunAsync(script, observer, cancellationToken);

            // Upload artifacts
            if (workspace.GetArtifacts().Any())
            {
                using (var tempWorkspace = new TestWorkspace())
                {
                    var archiveZip = Path.Combine(tempWorkspace.WorkingDirectory, "artifacts.zip");
                    ZipFile.CreateFromDirectory(workspace.WorkingDirectory, archiveZip);
                    await Client.UploadArtifactAsync(runnerId, response, new FileInfo(archiveZip), "artifacts.zip", accessToken);
                }
            }

            // Done
            response.Status = PipelineJobStatus.Completed;
            await Client.PostJobResponseAsync(runnerId, response, accessToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error");

            var response = new RunResponse { Guid = job.Guid, Accepted = true, Status = PipelineJobStatus.Error, ErrorMessage = ex.ToString() };
            await Client.PostJobResponseAsync(runnerId, response, accessToken);
        }
    }
}
