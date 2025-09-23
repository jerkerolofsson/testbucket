using System.Security.Claims;

using TestBucket.Contracts.Automation;
using TestBucket.Domain.Automation.Runners;
using TestBucket.Domain.Automation.Runners.Models;
using TestBucket.Domain.Identity;
using TestBucket.Domain.UnitTests.Automation.Fakes;

namespace TestBucket.Domain.UnitTests.Automation;

/// <summary>
/// Unit tests for <see cref="JobManager"/>.
/// </summary>
[UnitTest]
[EnrichedTest]
[FunctionalTest]
[Component("Automation")]
public class JobManagerTests
{
    private const string TENANT_ID = "tenant-1";

    /// <summary>
    /// Creates a <see cref="ClaimsPrincipal"/> for the test tenant.
    /// </summary>
    /// <returns>A <see cref="ClaimsPrincipal"/> representing the test tenant.</returns>
    private ClaimsPrincipal CreatePrincipal() => Impersonation.Impersonate(TENANT_ID);

    private readonly FakeTimeProvider _timeProvider = new FakeTimeProvider(new DateTimeOffset(2025, 09, 22, 3, 4, 5, TimeSpan.Zero));
    private readonly FakeJobRepository _repo = new();

    /// <summary>
    /// Creates a new instance of <see cref="JobManager"/> for testing.
    /// </summary>
    /// <returns>A <see cref="JobManager"/> instance.</returns>
    private JobManager CreateSut() => new JobManager(_repo, _timeProvider);

    /// <summary>
    /// Verifies that <see cref="JobManager.AddAsync"/> adds a job to the repository.
    /// </summary>
    [Fact]
    public async Task AddAsync_ShouldAddJobToRepository()
    {
        // Arrange
        var sut = CreateSut();
        var principal = CreatePrincipal();
        var job = new Job
        {
            Id = 1,
            Guid = Guid.NewGuid().ToString(),
            Priority = 1,
            Status = PipelineJobStatus.Created,
            Script = "echo Hello",
            Language = "bash",
            EnvironmentVariables = new Dictionary<string, string> { { "ENV", "test" } }
        };

        // Act
        await sut.AddAsync(principal, job);

        // Assert
        var result = await _repo.GetByIdAsync(job.Id);
        Assert.NotNull(result);
        Assert.Equal("echo Hello", result!.Script);
        Assert.Equal(job.Guid, result.Guid);
    }

    /// <summary>
    /// Verifies that <see cref="JobManager.GetByIdAsync"/> returns a job when it exists.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ShouldReturnJob_WhenJobExists()
    {
        // Arrange
        var sut = CreateSut();
        var principal = CreatePrincipal();
        var job = new Job
        {
            Id = 2,
            Guid = Guid.NewGuid().ToString(),
            Priority = 2,
            Status = PipelineJobStatus.Created,
            Script = "echo World",
            Language = "bash",
            EnvironmentVariables = new Dictionary<string, string> { { "ENV", "prod" } }
        };
        await sut.AddAsync(principal, job);

        // Act
        var result = await sut.GetByIdAsync(principal, job.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(job.Id, result!.Id);
        Assert.Equal("echo World", result.Script);
    }

    /// <summary>
    /// Verifies that <see cref="JobManager.GetByIdAsync"/> returns null when the job does not exist.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenJobDoesNotExist()
    {
        // Arrange
        var sut = CreateSut();
        var principal = CreatePrincipal();

        // Act
        var result = await sut.GetByIdAsync(principal, 999);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that <see cref="JobManager.GetJobByGuidAsync"/> returns a job when it exists.
    /// </summary>
    [Fact]
    public async Task GetJobByGuidAsync_ShouldReturnJob_WhenJobExists()
    {
        // Arrange
        var sut = CreateSut();
        var principal = CreatePrincipal();
        var job = new Job
        {
            Id = 3,
            Guid = Guid.NewGuid().ToString(),
            Priority = 3,
            Status = PipelineJobStatus.Created,
            Script = "echo Guid",
            Language = "bash",
            EnvironmentVariables = new Dictionary<string, string> { { "ENV", "guid" } }
        };
        await sut.AddAsync(principal, job);

        // Act
        var result = await sut.GetJobByGuidAsync(principal, job.Guid);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(job.Guid, result!.Guid);
    }

    /// <summary>
    /// Verifies that <see cref="JobManager.GetJobByGuidAsync"/> returns null when the job does not exist.
    /// </summary>
    [Fact]
    public async Task GetJobByGuidAsync_ShouldReturnNull_WhenJobDoesNotExist()
    {
        // Arrange
        var sut = CreateSut();
        var principal = CreatePrincipal();

        // Act
        var result = await sut.GetJobByGuidAsync(principal, "non-existent-guid");

        // Assert
        Assert.Null(result);
    }
}