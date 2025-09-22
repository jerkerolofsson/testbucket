using System.Security.Claims;

using TestBucket.Domain.Automation.Runners;
using TestBucket.Domain.Automation.Runners.Models;
using TestBucket.Domain.Identity;
using TestBucket.Domain.UnitTests.Automation.Fakes;

namespace TestBucket.Domain.UnitTests.Automation;

/// <summary>
/// Contains unit tests for the <see cref="RunnerManager"/> class, verifying runner management operations such as add, update, retrieve, and remove.
/// </summary>
[UnitTest]
[EnrichedTest]
[FunctionalTest]
[Component("Automation")]
public class RunnerManagerTests
{
    private const string TENANT_ID = "tenant-1";

    /// <summary>
    /// Creates a <see cref="ClaimsPrincipal"/> for the test tenant.
    /// </summary>
    /// <returns>A <see cref="ClaimsPrincipal"/> representing the test tenant.</returns>
    private ClaimsPrincipal CreatePrincipal() => Impersonation.Impersonate(TENANT_ID);

    private readonly FakeTimeProvider _timeProvider = new FakeTimeProvider(new DateTimeOffset(2025, 09, 22, 3, 4, 5, TimeSpan.Zero));
    private readonly FakeRunnerRepository _repo = new();

    /// <summary>
    /// Creates a new instance of <see cref="RunnerManager"/> for testing.
    /// </summary>
    /// <returns>A <see cref="RunnerManager"/> instance.</returns>
    private RunnerManager CreateSut() => new RunnerManager(_repo, _timeProvider);

    /// <summary>
    /// Creates a <see cref="Runner"/> instance with optional id and name.
    /// </summary>
    /// <param name="id">The runner id. Defaults to "runner-1".</param>
    /// <param name="name">The runner name. Defaults to "Test Runner".</param>
    /// <returns>A new <see cref="Runner"/> instance.</returns>
    private Runner CreateRunner(string id = "runner-1", string name = "Test Runner") => new Runner
    {
        Id = id,
        Name = name,
        Languages = new[] { "C#", "Python" },
        Tags = new[] { "tag1", "tag2" },
        PublicBaseUrl = "http://localhost"
    };

    /// <summary>
    /// Verifies that <see cref="RunnerManager.AddAsync"/> adds a runner for the tenant.
    /// </summary>
    [Fact]
    public async Task AddAsync_AddsRunner()
    {
        // Arrange
        var sut = CreateSut();
        var principal = CreatePrincipal();
        var runner = CreateRunner();

        // Act
        await sut.AddAsync(principal, runner);

        // Assert
        var all = await sut.GetAllAsync(principal);
        Assert.Single(all);
        Assert.Equal(runner.Id, all[0].Id);
    }

    /// <summary>
    /// Verifies that <see cref="RunnerManager.GetAllAsync"/> returns all runners for the tenant.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_ReturnsAllRunnersForTenant()
    {
        // Arrange
        var sut = CreateSut();
        var principal = CreatePrincipal();
        var runner1 = CreateRunner("runner-1", "Runner 1");
        var runner2 = CreateRunner("runner-2", "Runner 2");

        await sut.AddAsync(principal, runner1);
        await sut.AddAsync(principal, runner2);

        // Act
        var all = await sut.GetAllAsync(principal);

        // Assert
        Assert.Equal(2, all.Count);
        Assert.Contains(all, r => r.Id == "runner-1");
        Assert.Contains(all, r => r.Id == "runner-2");
    }

    /// <summary>
    /// Verifies that <see cref="RunnerManager.GetByIdAsync"/> returns the correct runner by id.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ReturnsRunnerById()
    {
        // Arrange
        var sut = CreateSut();
        var principal = CreatePrincipal();
        var runner = CreateRunner();

        await sut.AddAsync(principal, runner);

        // Act
        var found = await sut.GetByIdAsync(principal, runner.Id);

        // Assert
        Assert.NotNull(found);
        Assert.Equal(runner.Id, found!.Id);
    }

    /// <summary>
    /// Verifies that <see cref="RunnerManager.UpdateAsync"/> updates the runner's properties.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_UpdatesRunner()
    {
        // Arrange
        var sut = CreateSut();
        var principal = CreatePrincipal();
        var runner = CreateRunner();

        await sut.AddAsync(principal, runner);

        // Act
        runner.Name = "Updated Name";
        runner.Tags = new[] { "updated" };
        await sut.UpdateAsync(principal, runner);

        var updated = await sut.GetByIdAsync(principal, runner.Id);

        // Assert
        Assert.NotNull(updated);
        Assert.Equal("Updated Name", updated!.Name);
        Assert.Contains("updated", updated.Tags);
    }

    /// <summary>
    /// Verifies that <see cref="RunnerManager.RemoveAsync"/> removes the runner from the tenant.
    /// </summary>
    [Fact]
    public async Task RemoveAsync_RemovesRunner()
    {
        // Arrange
        var sut = CreateSut();
        var principal = CreatePrincipal();
        var runner = CreateRunner();

        await sut.AddAsync(principal, runner);

        // Act
        await sut.RemoveAsync(principal, runner);

        var all = await sut.GetAllAsync(principal);

        // Assert
        Assert.Empty(all);
    }
}