using System.Security.Claims;

using TestBucket.Contracts.Automation;
using TestBucket.Contracts.Testing.Models;
using TestBucket.Domain.Automation.Hybrid;
using TestBucket.Domain.Automation.Runners;
using TestBucket.Domain.Automation.Runners.Jobs;
using TestBucket.Domain.Automation.Runners.Models;
using TestBucket.Domain.Identity;
using TestBucket.Domain.UnitTests.Automation.Fakes;
using TestBucket.Formats;

namespace TestBucket.Domain.UnitTests.Automation.Hybrid;

/// <summary>
/// Contains unit tests for the <see cref="HybridRunner"/> class, verifying job creation and supported language logic.
/// </summary>
[UnitTest]
[EnrichedTest]
[FunctionalTest]
[Component("Automation")]
[Feature("Hybrid Tests")]
public class HybridRunnerTests
{
    private const string TENANT_ID = "tenant-1";

    /// <summary>
    /// Creates a <see cref="ClaimsPrincipal"/> for the test tenant.
    /// </summary>
    /// <returns>A <see cref="ClaimsPrincipal"/> representing the test tenant.</returns>
    private ClaimsPrincipal CreatePrincipal() => Impersonation.Impersonate(TENANT_ID);

    /// <summary>
    /// Creates a <see cref="ClaimsPrincipal"/> for a specific project within the test tenant.
    /// </summary>
    /// <param name="projectId">The project ID to associate with the principal.</param>
    /// <returns>A <see cref="ClaimsPrincipal"/> representing the test tenant and project.</returns>
    private ClaimsPrincipal CreatePrincipalForProject(long projectId) => Impersonation.Impersonate(x =>
    {
        x.ProjectId = projectId;
        x.TenantId = TENANT_ID;
        x.UserName = x.Email = "user@sunet.se";
        x.AddAllPermissions();
    });

    private readonly FakeTimeProvider _timeProvider = new FakeTimeProvider(new DateTimeOffset(2025, 09, 22, 3, 4, 5, TimeSpan.Zero));
    private readonly FakeJobRepository _jobRepo = new();
    private readonly FakeRunnerRepository _runnerRepo = new();

    /// <summary>
    /// Creates a new instance of <see cref="JobManager"/> for testing.
    /// </summary>
    /// <returns>A <see cref="JobManager"/> instance.</returns>
    private JobManager CreateJobManager() => new JobManager(_jobRepo, _timeProvider);

    /// <summary>
    /// Creates a new instance of <see cref="HybridRunner"/> for testing.
    /// </summary>
    /// <param name="jobAddedEventSignal">The event signal to use for job addition notifications.</param>
    /// <returns>A <see cref="HybridRunner"/> instance.</returns>
    private HybridRunner CreateSut(JobAddedEventSignal jobAddedEventSignal) => new HybridRunner(_runnerRepo, CreateJobManager(), jobAddedEventSignal);

    /// <summary>
    /// Verifies that a job is created correctly, copying information from the <see cref="TestExecutionContext"/>.
    /// </summary>
    [Fact]
    public void CreateJob_Should_Map_Fields_Correctly()
    {
        // Arrange
        var context = new TestExecutionContext
        {
            Guid = Guid.NewGuid().ToString(),
            ProjectId = 1,
            TeamId = 2,
            TenantId = TENANT_ID,
            TestRunId = 123,
            TestSuiteId = 456,
            TestSuiteName = "Suite",
            TestCaseId = 789,
            TestEnvironmentId = 321,
            // Add a Variables property if it exists in your context
            Variables = new Dictionary<string, string> { { "ENV", "value" } }
        };
        string language = "csharp";
        string code = "Console.WriteLine(\"Hello World\");";

        // Act
        var job = HybridRunner.CreateJob(context, language, code);

        // Assert
        Assert.NotNull(job);
        Assert.Equal(context.TestRunId, job.TestRunId);
        Assert.Equal(language, job.Language);
        Assert.Equal(code, job.Script);
        Assert.Equal(1, job.Priority);
        Assert.Equal(PipelineJobStatus.Queued, job.Status);
        Assert.Equal(context.Variables, job.EnvironmentVariables);
        Assert.False(string.IsNullOrWhiteSpace(job.Guid));
    }

    /// <summary>
    /// Verifies that <see cref="HybridRunner.GetSupportedLanguagesAsync"/> returns an empty array when no runners are present.
    /// </summary>
    [Fact]
    public async Task GetSupportedLanguagesAsync_Returns_Empty_When_No_Runners()
    {
        // Arrange
        var principal = CreatePrincipal();
        var runnerRepo = new FakeRunnerRepository();
        var sut = new HybridRunner(runnerRepo, CreateJobManager(), new JobAddedEventSignal());

        // Act
        var result = await sut.GetSupportedLanguagesAsync(principal);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    /// <summary>
    /// Verifies that GetSupportedLanguagesAsync excludes languages from runners with a different tenant ID.
    /// </summary>
    [Fact]
    public async Task GetSupportedLanguagesAsync_Excludes_Languages_From_Different_Tenant()
    {
        // Arrange
        var principal = CreatePrincipal();
        var runnerRepo = new FakeRunnerRepository();
        // Runner with matching tenant
        await runnerRepo.AddAsync(new Runner { Id = "1", Name = "A", Languages = new[] { "python" }, Tags = Array.Empty<string>(), TenantId = TENANT_ID });
        // Runner with different tenant
        await runnerRepo.AddAsync(new Runner { Id = "2", Name = "B", Languages = new[] { "csharp" }, Tags = Array.Empty<string>(), TenantId = "other-tenant" });
        var sut = new HybridRunner(runnerRepo, CreateJobManager(), new JobAddedEventSignal());

        // Act
        var result = await sut.GetSupportedLanguagesAsync(principal);

        // Assert
        Assert.Contains("python", result);
        Assert.DoesNotContain("csharp", result);
        Assert.Single(result);
    }

    /// <summary>
    /// Verifies that <see cref="HybridRunner.GetSupportedLanguagesAsync"/> returns all unique languages from all runners.
    /// </summary>
    [Fact]
    public async Task GetSupportedLanguagesAsync_Returns_All_Languages_From_Runners()
    {
        // Arrange
        var principal = CreatePrincipal();
        var runnerRepo = new FakeRunnerRepository();
        await runnerRepo.AddAsync(new Runner { Id = "1", Name = "A", Languages = new[] { "python", "csharp" }, Tags = Array.Empty<string>(), TenantId = TENANT_ID });
        await runnerRepo.AddAsync(new Runner { Id = "2", Name = "B", Languages = new[] { "javascript", "python" }, Tags = Array.Empty<string>(), TenantId = TENANT_ID });
        var sut = new HybridRunner(runnerRepo, CreateJobManager(), new JobAddedEventSignal());

        // Act
        var result = await sut.GetSupportedLanguagesAsync(principal);

        // Assert
        Assert.Contains("python", result);
        Assert.Contains("csharp", result);
        Assert.Contains("javascript", result);
        Assert.Equal(3, result.Length);
    }

    /// <summary>
    /// Verifies that <see cref="HybridRunner.GetSupportedLanguagesAsync"/> ignores runners with null languages.
    /// </summary>
    [Fact]
    public async Task GetSupportedLanguagesAsync_Ignores_Runners_With_Null_Languages()
    {
        // Arrange
        var principal = CreatePrincipal();
        var runnerRepo = new FakeRunnerRepository();
        await runnerRepo.AddAsync(new Runner { Id = "1", Name = "A", Languages = null, Tags = Array.Empty<string>(), TenantId = TENANT_ID });
        await runnerRepo.AddAsync(new Runner { Id = "2", Name = "B", Languages = new[] { "powershell" }, Tags = Array.Empty<string>(), TenantId = TENANT_ID });
        var sut = new HybridRunner(runnerRepo, CreateJobManager(), new JobAddedEventSignal());

        // Act
        var result = await sut.GetSupportedLanguagesAsync(principal);

        // Assert
        Assert.Single(result);
        Assert.Contains("powershell", result);
    }

    /// <summary>
    /// Verifies that <see cref="HybridRunner.GetSupportedLanguagesAsync"/> respects the project ID filter when returning languages.
    /// </summary>
    [Fact]
    public async Task GetSupportedLanguagesAsync_Respects_ProjectId_Filter()
    {
        // Arrange
        var projectId = 42L;
        var principal = CreatePrincipalForProject(projectId);
        var runnerRepo = new FakeRunnerRepository();
        await runnerRepo.AddAsync(new Runner { Id = "1", Name = "A", Languages = new[] { "python" }, Tags = Array.Empty<string>(), TestProjectId = projectId, TenantId = TENANT_ID });
        await runnerRepo.AddAsync(new Runner { Id = "2", Name = "B", Languages = new[] { "csharp" }, Tags = Array.Empty<string>(), TestProjectId = 999L, TenantId = TENANT_ID });
        await runnerRepo.AddAsync(new Runner { Id = "3", Name = "C", Languages = new[] { "javascript" }, Tags = Array.Empty<string>(), TestProjectId = null, TenantId = TENANT_ID });
        var sut = new HybridRunner(runnerRepo, CreateJobManager(), new JobAddedEventSignal());

        // Act
        var result = await sut.GetSupportedLanguagesAsync(principal);

        // Assert
        Assert.Contains("python", result);
        Assert.Contains("javascript", result); // null projectId means global
        Assert.DoesNotContain("csharp", result);
        Assert.Equal(2, result.Length);
    }


    /// <summary>
    /// Verifies that <see cref="HybridRunner.RunAsync"/> creates a job, signals job addition, and returns a result when the job completes successfully.
    /// </summary>
    [Theory]
    [InlineData(PipelineJobStatus.Failed)]
    [InlineData(PipelineJobStatus.Canceled)]
    [InlineData(PipelineJobStatus.Skipped)]
    [InlineData(PipelineJobStatus.Completed)]
    public async Task RunAsync_Creates_Job_And_Returns_Failed_Result(PipelineJobStatus status)
    {
        // Arrange
        var principal = CreatePrincipal();
        var context = new TestExecutionContext
        {
            TeamId = 2,
            ProjectId = 1,
            Guid = Guid.NewGuid().ToString(),
            TestRunId = 1,
            TestSuiteId = 2,
            TestSuiteName = "Suite",
            TestCaseId = 3,
            TestEnvironmentId = 4,
            Variables = new Dictionary<string, string> { { "ENV", "value" } }
        };
        string language = "csharp";
        string code = "Console.WriteLine(\"Hello World\");";
        var jobAddedSignal = new JobAddedEventSignal();
        var jobRepo = new FakeJobRepository();
        var jobManager = new JobManager(jobRepo, _timeProvider);
        var sut = new HybridRunner(_runnerRepo, jobManager, jobAddedSignal);

        // Act
        var runTask = Task.Run(async () => await sut.RunAsync(principal, context, language, code, CancellationToken.None));

        // Synchronize
        await jobAddedSignal.WaitAsync();

        // Simulate job completion
        var createdJob = await jobRepo.GetByGuidAsync(jobAddedSignal.JobGuid!);
        Assert.NotNull(createdJob);
        if (createdJob != null)
        {
            createdJob.Status = status;
            createdJob.StdOut = "Test output";
            createdJob.Result = "<result/>";
            createdJob.Format = TestResultFormat.JUnitXml;
            await jobRepo.UpdateAsync(createdJob);
        }

        var result = await runTask;

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal("Test output", result.StdOut);
        Assert.Equal("<result/>", result.Result);
        Assert.Equal(TestResultFormat.JUnitXml, result.Format);
        Assert.True(result.Completed);
    }

    /// <summary>
    /// Verifies that <see cref="HybridRunner.RunAsync"/> creates a job, signals job addition, and returns a result when the job completes successfully.
    /// </summary>
    [Fact]
    public async Task RunAsync_Creates_Job_And_Returns_Success_Result()
    {
        // Arrange
        var principal = CreatePrincipal();
        var context = new TestExecutionContext
        {
            TeamId = 2,
            ProjectId = 1,
            Guid = Guid.NewGuid().ToString(),
            TestRunId = 1,
            TestSuiteId = 2,
            TestSuiteName = "Suite",
            TestCaseId = 3,
            TestEnvironmentId = 4,
            Variables = new Dictionary<string, string> { { "ENV", "value" } }
        };
        string language = "csharp";
        string code = "Console.WriteLine(\"Hello World\");";
        var jobAddedSignal = new JobAddedEventSignal();
        var jobRepo = new FakeJobRepository();
        var jobManager = new JobManager(jobRepo, _timeProvider);
        var sut = new HybridRunner(_runnerRepo, jobManager, jobAddedSignal);

        // Act
        var runTask = Task.Run(async () => await sut.RunAsync(principal, context, language, code, CancellationToken.None));

        // Synchronize
        await jobAddedSignal.WaitAsync();

        // Simulate job completion
        var createdJob = await jobRepo.GetByGuidAsync(jobAddedSignal.JobGuid!);
        Assert.NotNull(createdJob);
        if (createdJob != null)
        {
            createdJob.Status = PipelineJobStatus.Success;
            createdJob.StdOut = "Test output";
            createdJob.Result = "<result/>";
            createdJob.Format = TestResultFormat.JUnitXml;
            await jobRepo.UpdateAsync(createdJob);
        }

        var result = await runTask;

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal("Test output", result.StdOut);
        Assert.Equal("<result/>", result.Result);
        Assert.Equal(TestResultFormat.JUnitXml, result.Format);
        Assert.True(result.Completed);
    }
}