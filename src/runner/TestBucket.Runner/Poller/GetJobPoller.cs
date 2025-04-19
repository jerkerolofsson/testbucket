using System.IO.Compression;
using System.Reflection.Metadata;

using TestBucket.Contracts.Automation;
using TestBucket.Contracts.Runners.Models;
using TestBucket.Formats;
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

            //await File.WriteAllTextAsync(Path.Combine(workspace.WorkingDirectory, "runner.stdout.txt"), observer.StdOut);
            //await File.WriteAllTextAsync(Path.Combine(workspace.WorkingDirectory, "runner.stderr.txt"), observer.StdErr);

            var artifacts = workspace.GetArtifacts();
            if (artifacts.Any())
            {
                // Upload artifacts if a test run id was specified
                if (job.TestRunId is not null)
                {
                    using (var tempWorkspace = new TestWorkspace())
                    {
                        var archiveZip = Path.Combine(tempWorkspace.WorkingDirectory, "artifacts.zip");
                        ZipFile.CreateFromDirectory(workspace.WorkingDirectory, archiveZip);
                        await Client.UploadArtifactAsync(runnerId, response, new FileInfo(archiveZip), "artifacts.zip", accessToken);
                    }
                }

                await AssignResultAndFormat(workspace, response);
            }

            // Done
            response.Status = PipelineJobStatus.Completed;
            response.StdOut = observer.StdOut;
            response.StdErr = observer.StdErr;
            await Client.PostJobResponseAsync(runnerId, response, accessToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error");

            var response = new RunResponse { Guid = job.Guid, Accepted = true, Status = PipelineJobStatus.Error, ErrorMessage = ex.ToString() };
            await Client.PostJobResponseAsync(runnerId, response, accessToken);
        }
    }

    private static async Task AssignResultAndFormat(TestWorkspace workspace, RunResponse response)
    {
        var testResultsArtifactsPattern = "**/*.trx;**/*.xml;**/*.json;**/*.txt";
        foreach (var file in workspace.GetArtifacts(testResultsArtifactsPattern))
        {
            var format = await TestResultDetector.DetectFromFileAsync(file.FullName);
            if (format != Formats.TestResultFormat.UnknownFormat)
            {
                response.Result = File.ReadAllText(file.FullName);
                response.Format = format;
                break;
            }
        }
    }
}
