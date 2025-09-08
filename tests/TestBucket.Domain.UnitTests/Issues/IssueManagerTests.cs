using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

using Mediator;

using Microsoft.Extensions.Logging;

using NSubstitute;

using TestBucket.Contracts.Issues.States;
using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Issues;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Issues.Search;
using TestBucket.Domain.UnitTests.Issues.Fakes;

using Xunit;

namespace TestBucket.Domain.UnitTests.Issues;

/// <summary>
/// Unit tests for the <see cref="IssueManager"/> class.
/// </summary>
[FunctionalTest]
[EnrichedTest]
[UnitTest]
[Component("Issues")]
public class IssueManagerTests
{
    private readonly IMediator _mediator;
    private readonly ILogger<IssueManager> _logger;
    private readonly IFieldDefinitionManager _fieldDefinitionManager;
    private readonly TimeProvider _timeProvider = new FakeTimeProvider(new DateTimeOffset(2025, 9, 8, 1, 2, 3, TimeSpan.Zero));

    /// <summary>
    /// Initializes a new instance of the <see cref="IssueManagerTests"/> class.
    /// </summary>
    public IssueManagerTests()
    {
        _mediator = Substitute.For<IMediator>();
        _logger = Substitute.For<ILogger<IssueManager>>();
        _fieldDefinitionManager = Substitute.For<IFieldDefinitionManager>();
    }

    /// <summary>
    /// Creates a <see cref="ClaimsPrincipal"/> with the specified permission level.
    /// </summary>
    /// <param name="level">The permission level to assign to the principal.</param>
    /// <returns>A <see cref="ClaimsPrincipal"/> with the specified permissions.</returns>
    private ClaimsPrincipal CreatePrincipal(PermissionLevel level)
    {
        return Impersonation.Impersonate(builder =>
        {
            builder.TenantId = "tenant-1";
            builder.UserName = "user1@testbucket.io";
            builder.Add(PermissionEntityType.Issue, level);
        });
    }

    /// <summary>
    /// Tests that <see cref="IssueManager.AddLocalIssueAsync"/> adds a local issue successfully.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task AddLocalIssueAsync_ShouldAddIssue()
    {
        // Arrange
        var principal = CreatePrincipal(PermissionLevel.ReadWrite);
        var issue = new LocalIssue
        {
            Title = "Test Issue",
            Description = "Test Description",
            TestProjectId = 1
        };
        var issueManager = new IssueManager(new FakeIssueRepository(), _mediator, _fieldDefinitionManager, _logger, _timeProvider);

        // Act
        await issueManager.AddLocalIssueAsync(principal, issue);

        // Assert
        var addedIssue = await issueManager.GetIssueByIdAsync(principal, issue.Id);
        Assert.NotNull(addedIssue);
        Assert.Equal("Test Issue", addedIssue.Title);
    }

    /// <summary>
    /// Tests that <see cref="IssueManager.UpdateLocalIssueAsync"/> updates a local issue successfully.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task UpdateLocalIssueAsync_ShouldUpdateIssue()
    {
        // Arrange
        var principal = CreatePrincipal(PermissionLevel.ReadWrite);
        var issue = new LocalIssue
        {
            Title = "Original Title",
            Description = "Original Description",
            TestProjectId = 1
        };
        var issueManager = new IssueManager(new FakeIssueRepository(), _mediator, _fieldDefinitionManager, _logger, _timeProvider);
        await issueManager.AddLocalIssueAsync(principal, issue);

        // Act
        issue.Title = "Updated Title";
        issue.Description = "Updated Description";
        await issueManager.UpdateLocalIssueAsync(principal, issue);

        // Assert
        var updatedIssue = await issueManager.GetIssueByIdAsync(principal, issue.Id);
        Assert.NotNull(updatedIssue);
        Assert.Equal("Updated Title", updatedIssue.Title);
        Assert.Equal("Updated Description", updatedIssue.Description);
    }

    /// <summary>
    /// Tests that <see cref="IssueManager.DeleteLocalIssueAsync"/> removes a local issue successfully.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task DeleteLocalIssueAsync_ShouldRemoveIssue()
    {
        // Arrange
        var principal = CreatePrincipal(PermissionLevel.All);
        var issue = new LocalIssue
        {
            Title = "Issue to Delete",
            Description = "Description",
            TestProjectId = 1
        };
        var issueManager = new IssueManager(new FakeIssueRepository(), _mediator, _fieldDefinitionManager, _logger, _timeProvider);
        await issueManager.AddLocalIssueAsync(principal, issue);

        // Act
        await issueManager.DeleteLocalIssueAsync(principal, issue);

        // Assert
        var deletedIssue = await issueManager.GetIssueByIdAsync(principal, issue.Id);
        Assert.Null(deletedIssue);
    }

    /// <summary>
    /// Tests that <see cref="IssueManager.FindLocalIssueAsync"/> returns the correct issue.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task FindLocalIssueAsync_ShouldReturnCorrectIssue()
    {
        // Arrange
        var principal = CreatePrincipal(PermissionLevel.ReadWrite);
        var issue = new LocalIssue
        {
            Title = "Findable Issue",
            Description = "Description",
            TestProjectId = 1,
            ExternalDisplayId = "ISSUE-123"
        };
        var issueManager = new IssueManager(new FakeIssueRepository(), _mediator, _fieldDefinitionManager, _logger, _timeProvider);
        await issueManager.AddLocalIssueAsync(principal, issue);

        // Act
        var foundIssue = await issueManager.FindLocalIssueAsync(principal, issue.TestProjectId.Value, "ISSUE-123");

        // Assert
        Assert.NotNull(foundIssue);
        Assert.Equal(issue.Title, foundIssue?.Title);
    }

    /// <summary>
    /// Tests that IssueManager.SearchLocalIssuesAsync returns matching issues.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task SearchLocalIssuesAsync_ShouldReturnMatchingIssues()
    {
        // Arrange
        var principal = CreatePrincipal(PermissionLevel.ReadWrite);
        var issue1 = new LocalIssue
        {
            Title = "Searchable Issue 1",
            Description = "Description 1",
            TestProjectId = 1
        };
        var issue2 = new LocalIssue
        {
            Title = "Searchable Issue 2",
            Description = "Description 2",
            TestProjectId = 1
        };
        var issueManager = new IssueManager(new FakeIssueRepository(), _mediator, _fieldDefinitionManager, _logger, _timeProvider);
        await issueManager.AddLocalIssueAsync(principal, issue1);
        await issueManager.AddLocalIssueAsync(principal, issue2);

        // Act
        var searchResults = await issueManager.SearchLocalIssuesAsync(principal, 1, "Searchable", 0, 10);

        // Assert
        Assert.NotNull(searchResults);
        Assert.Equal(2, searchResults.Items.Length);
        Assert.Contains(searchResults.Items, i => i.Title == "Searchable Issue 1");
        Assert.Contains(searchResults.Items, i => i.Title == "Searchable Issue 2");
    }

    /// <summary>
    /// Tests that IssueManager.AddObserver and IssueManager.RemoveObserver manage observers correctly with ILocalIssueObserver.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public void AddAndRemoveLocalIssueObserver_ShouldManageObserversCorrectly()
    {
        // Arrange
        var observer = Substitute.For<ILocalIssueObserver>();
        var issueManager = new IssueManager(new FakeIssueRepository(), _mediator, _fieldDefinitionManager, _logger, _timeProvider);

        // Act
        issueManager.AddObserver(observer);
        issueManager.RemoveObserver(observer);

        // Assert
        Assert.Empty(issueManager.LocalIssueObservers);
    }

    /// <summary>
    /// Tests that IssueManager.AddObserver and IssueManager.RemoveObserver manage observers correctly with ILinkedIssueObserver.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public void AddAndRemoveLinkedIssueObserver_ShouldManageObserversCorrectly()
    {
        // Arrange
        var observer = Substitute.For<ILinkedIssueObserver>();
        var issueManager = new IssueManager(new FakeIssueRepository(), _mediator, _fieldDefinitionManager, _logger, _timeProvider);

        // Act
        issueManager.AddObserver(observer);
        issueManager.RemoveObserver(observer);

        // Assert
        Assert.Empty(issueManager.LocalIssueObservers);
    }

    /// <summary>
    /// Tests that the Created and Modified timestamps are set when adding a local issue.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task AddLocalIssueAsync_ShouldSetCreatedAndModifiedTimestamps()
    {
        // Arrange
        var principal = CreatePrincipal(PermissionLevel.ReadWrite);
        var issue = new LocalIssue
        {
            Title = "Timestamp Test Issue",
            Description = "Testing Created and Modified timestamps",
            TestProjectId = 1
        };
        var issueManager = new IssueManager(new FakeIssueRepository(), _mediator, _fieldDefinitionManager, _logger, _timeProvider);

        // Act
        await issueManager.AddLocalIssueAsync(principal, issue);

        // Assert
        var addedIssue = await issueManager.GetIssueByIdAsync(principal, issue.Id);
        Assert.NotNull(addedIssue);
        Assert.Equal(_timeProvider.GetUtcNow(), addedIssue.Created);
        Assert.Equal(_timeProvider.GetUtcNow(), addedIssue.Modified);
    }

    /// <summary>
    /// Tests that the Modified timestamp is updated when updating a local issue.
    /// </summary>
    [Fact]
    [FunctionalTest]
    public async Task UpdateLocalIssueAsync_ShouldUpdateModifiedTimestamp()
    {
        // Arrange
        var principal = CreatePrincipal(PermissionLevel.ReadWrite);
        var issue = new LocalIssue
        {
            Title = "Original Title",
            Description = "Original Description",
            TestProjectId = 1
        };
        var issueManager = new IssueManager(new FakeIssueRepository(), _mediator, _fieldDefinitionManager, _logger, _timeProvider);
        await issueManager.AddLocalIssueAsync(principal, issue);

        // Act
        issue.Title = "Updated Title";
        issue.Description = "Updated Description";
        issue.Modified = _timeProvider.GetUtcNow().AddMinutes(5); // Set it to something which isn't the same as timeprovider
        await issueManager.UpdateLocalIssueAsync(principal, issue);

        // Assert
        var updatedIssue = await issueManager.GetIssueByIdAsync(principal, issue.Id);
        Assert.NotNull(updatedIssue);
        Assert.Equal("Updated Title", updatedIssue.Title);
        Assert.Equal("Updated Description", updatedIssue.Description);
        Assert.Equal(_timeProvider.GetUtcNow(), updatedIssue.Modified);
    }

    /// <summary>
    /// Tests that <see cref="IssueManager.DeleteLocalIssueAsync"/> throws an exception when the user lacks sufficient permissions.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task DeleteLocalIssueAsync_ShouldThrowWhenUserLacksPermissions()
    {
        // Arrange
        var admin = CreatePrincipal(PermissionLevel.All); // Insufficient permissions
        var user = CreatePrincipal(PermissionLevel.Read); // Insufficient permissions
        var issue = new LocalIssue
        {
            Title = "Issue",
            Description = "Hello",
            TestProjectId = 1
        };
        var issueManager = new IssueManager(new FakeIssueRepository(), _mediator, _fieldDefinitionManager, _logger, _timeProvider);
        await issueManager.AddLocalIssueAsync(admin, issue);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => issueManager.DeleteLocalIssueAsync(user, issue));
    }

    /// <summary>
    /// Tests that <see cref="IssueManager.DeleteLocalIssueAsync"/> throws an exception when the user lacks sufficient permissions.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task UpdateLocalIssueAsync_ShouldThrowWhenUserLacksPermissions()
    {
        // Arrange
        var admin = CreatePrincipal(PermissionLevel.All); // Insufficient permissions
        var user = CreatePrincipal(PermissionLevel.Read); // Insufficient permissions
        var issue = new LocalIssue
        {
            Title = "Issue",
            Description = "Hello",
            TestProjectId = 1
        };
        var issueManager = new IssueManager(new FakeIssueRepository(), _mediator, _fieldDefinitionManager, _logger, _timeProvider);
        await issueManager.AddLocalIssueAsync(admin, issue);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => issueManager.UpdateLocalIssueAsync(user, issue));
    }

    /// <summary>
    /// Tests that <see cref="IssueManager.AddLocalIssueAsync"/> throws an exception when the user lacks sufficient permissions.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task AddLocalIssueAsync_ShouldThrowWhenUserLacksPermissions()
    {
        // Arrange
        var principal = CreatePrincipal(PermissionLevel.Read); // Insufficient permissions
        var issue = new LocalIssue
        {
            Title = "Unauthorized Issue",
            Description = "This should fail",
            TestProjectId = 1
        };
        var issueManager = new IssueManager(new FakeIssueRepository(), _mediator, _fieldDefinitionManager, _logger, _timeProvider);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => issueManager.AddLocalIssueAsync(principal, issue));
    }

    /// <summary>
    /// Tests that <see cref="IssueManager.AddLocalIssueAsync"/> succeeds when the user has sufficient permissions.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task AddLocalIssueAsync_ShouldSucceedWhenUserHasPermissions()
    {
        // Arrange
        var principal = CreatePrincipal(PermissionLevel.ReadWrite); // Sufficient permissions
        var issue = new LocalIssue
        {
            Title = "Authorized Issue",
            Description = "This should succeed",
            TestProjectId = 1
        };
        var issueManager = new IssueManager(new FakeIssueRepository(), _mediator, _fieldDefinitionManager, _logger, _timeProvider);

        // Act
        await issueManager.AddLocalIssueAsync(principal, issue);

        // Assert
        var addedIssue = await issueManager.GetIssueByIdAsync(principal, issue.Id);
        Assert.NotNull(addedIssue);
        Assert.Equal("Authorized Issue", addedIssue.Title);
    }
}