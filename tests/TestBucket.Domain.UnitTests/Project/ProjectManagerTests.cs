using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Projects.Models;
using Xunit;
using TestBucket.Domain.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using Mediator;
using Microsoft.Extensions.Caching.Memory;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Projects.Events;
using NSubstitute;

namespace TestBucket.Domain.UnitTests.Project;

/// <summary>
/// Unit tests for the <see cref="ProjectManager"/> class.
/// </summary>
[EnrichedTest]
[UnitTest]
[Component("Projects")]
public class ProjectManagerTests
{
    private const string TenantId = "tenant-1";
    private const string UserName = "admin@admin.com";

    /// <summary>
    /// Tests that <see cref="ProjectManager.AddAsync"/> adds a project successfully.
    /// </summary>
    [Fact]
    [FunctionalTest]

    public async Task AddAsync_ShouldAddProjectSuccessfully()
    {
        var mediator = NSubstitute.Substitute.For<IMediator>();
        var memoryCache = NSubstitute.Substitute.For<IMemoryCache>();

        // Arrange
        var fakeRepository = new FakeProjectRepository();
        var projectManager = new ProjectManager(fakeRepository, memoryCache, mediator, NullLogger<ProjectManager>.Instance);
        var principal = Impersonation.Impersonate(TenantId);
        var project = new TestProject { Name = "Test Project", Slug = "test-project", ShortName = "TP" };

        // Act
        var result = await projectManager.AddAsync(principal, project);

        // Assert
        Assert.True(result.IsT0);
        Assert.NotNull(result.AsT0);
        Assert.Equal("Test Project", result.AsT0.Name);
        Assert.Equal(TenantId, result.AsT0.TenantId);
    }

    /// <summary>
    /// Tests that <see cref="ProjectManager.GetTestProjectByIdAsync"/> retrieves a project by its ID.
    /// </summary>
    [Fact]
    [FunctionalTest]

    public async Task GetTestProjectByIdAsync_ShouldReturnProject()
    {
        var mediator = NSubstitute.Substitute.For<IMediator>();
        var memoryCache = NSubstitute.Substitute.For<IMemoryCache>();

        // Arrange
        var fakeRepository = new FakeProjectRepository();
        var projectManager = new ProjectManager(fakeRepository, memoryCache, mediator, NullLogger<ProjectManager>.Instance);
        var principal = Impersonation.Impersonate(TenantId);
        var project = new TestProject { Name = "Test Project", Id = 1, TenantId = TenantId, Slug = "test-project", ShortName = "TP" };
        await fakeRepository.AddAsync(project);

        // Act
        var retrievedProject = await projectManager.GetTestProjectByIdAsync(principal, 1);

        // Assert
        Assert.NotNull(retrievedProject);
        Assert.Equal("Test Project", retrievedProject.Name);
    }

    /// <summary>
    /// Tests that ProjectManager.DeleteAsync removes a project successfully.
    /// </summary>
    [Fact]
    [FunctionalTest]

    public async Task DeleteAsync_ShouldRemoveProject()
    {
        // Arrange
        var mediator = NSubstitute.Substitute.For<IMediator>();
        var memoryCache = NSubstitute.Substitute.For<IMemoryCache>();

        var fakeRepository = new FakeProjectRepository();
        var projectManager = new ProjectManager(fakeRepository, memoryCache, mediator, NullLogger<ProjectManager>.Instance);
        var principal = Impersonation.Impersonate(TenantId);
        var project = new TestProject { Name = "Test Project", Id = 1, TenantId = TenantId, Slug = "test-project", ShortName = "TP" };
        await fakeRepository.AddAsync(project);

        // Act
        await projectManager.DeleteAsync(principal, project);

        // Assert
        var retrievedProject = await fakeRepository.GetProjectByIdAsync("tenant1", 1);
        Assert.Null(retrievedProject);
    }

    /// <summary>
    /// Tests that ProjectManager.DeleteAsync throws an exception when the tenant is incorrect.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task DeleteAsync_WithWrongTenant_ShouldThrow()
    {
        // Arrange
        var mediator = NSubstitute.Substitute.For<IMediator>();
        var memoryCache = NSubstitute.Substitute.For<IMemoryCache>();

        var fakeRepository = new FakeProjectRepository();
        var projectManager = new ProjectManager(fakeRepository, memoryCache, mediator, NullLogger<ProjectManager>.Instance);
        var principal = Impersonation.Impersonate(TenantId);
        var project = new TestProject { Name = "Test Project", Id = 1, TenantId = "tenant-2", Slug = "test-project", ShortName = "TP" };
        await fakeRepository.AddAsync(project);

        // Act
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await projectManager.DeleteAsync(principal, project));
    }

    /// <summary>
    /// Tests that <see cref="ProjectManager.AddAsync"/> adds a project successfully when the user has correct permissions.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task AddAsync_WithCorrectPermissions_ShouldAddProjectSuccessfully()
    {
        // Arrange
        var mediator = NSubstitute.Substitute.For<IMediator>();
        var memoryCache = NSubstitute.Substitute.For<IMemoryCache>();
        var fakeRepository = new FakeProjectRepository();
        var projectManager = new ProjectManager(fakeRepository, memoryCache, mediator, NullLogger<ProjectManager>.Instance);

        var principal = Impersonation.Impersonate(builder =>
        {
            builder.TenantId = TenantId;
            builder.UserName = UserName;
            builder.Email = UserName;
            builder.Add(PermissionEntityType.Project, PermissionLevel.Write);
        });

        var project = new TestProject { Name = "Test Project", Slug = "test-project", ShortName = "TP" };

        // Act
        var result = await projectManager.AddAsync(principal, project);

        // Assert
        Assert.True(result.IsT0);
        Assert.NotNull(result.AsT0);
        Assert.Equal("Test Project", result.AsT0.Name);
    }

    /// <summary>
    /// Tests that <see cref="ProjectManager.AddAsync"/> throws an exception when the user lacks correct permissions.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task AddAsync_WithoutCorrectPermissions_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var mediator = NSubstitute.Substitute.For<IMediator>();
        var memoryCache = NSubstitute.Substitute.For<IMemoryCache>();
        var fakeRepository = new FakeProjectRepository();
        var projectManager = new ProjectManager(fakeRepository, memoryCache, mediator, NullLogger<ProjectManager>.Instance);

        var principal = Impersonation.Impersonate(builder =>
        {
            builder.TenantId = TenantId;
            builder.UserName = UserName;
            builder.Email = UserName;
            builder.Add(PermissionEntityType.Project, PermissionLevel.Read);
        });

        var project = new TestProject { Name = "Test Project", Slug = "test-project", ShortName = "TP" };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await projectManager.AddAsync(principal, project));
    }

    /// <summary>
    /// Tests that ProjectManager.DeleteAsync removes a project successfully when the user has correct permissions.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task DeleteAsync_WithCorrectPermissions_ShouldRemoveProjectSuccessfully()
    {
        // Arrange
        var mediator = NSubstitute.Substitute.For<IMediator>();
        var memoryCache = NSubstitute.Substitute.For<IMemoryCache>();
        var fakeRepository = new FakeProjectRepository();
        var projectManager = new ProjectManager(fakeRepository, memoryCache, mediator, NullLogger<ProjectManager>.Instance);

        var principal = Impersonation.Impersonate(builder =>
        {
            builder.TenantId = TenantId;
            builder.UserName = UserName;
            builder.Email = UserName;
            builder.Add(PermissionEntityType.Project, PermissionLevel.Delete);
        });

        var project = new TestProject { Name = "Test Project", Id = 1, TenantId = TenantId, Slug = "test-project", ShortName = "TP" };
        await fakeRepository.AddAsync(project);

        // Act
        await projectManager.DeleteAsync(principal, project);

        // Assert
        var retrievedProject = await fakeRepository.GetProjectByIdAsync(TenantId, 1);
        Assert.Null(retrievedProject);
    }

    /// <summary>
    /// Tests that ProjectManager.DeleteAsync throws an exception when the user lacks correct permissions.
    /// </summary>
    [Fact]
    [SecurityTest]
    public async Task DeleteAsync_WithoutCorrectPermissions_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var mediator = NSubstitute.Substitute.For<IMediator>();
        var memoryCache = NSubstitute.Substitute.For<IMemoryCache>();
        var fakeRepository = new FakeProjectRepository();
        var projectManager = new ProjectManager(fakeRepository, memoryCache, mediator, NullLogger<ProjectManager>.Instance);

        var principal = Impersonation.Impersonate(builder =>
        {
            builder.TenantId = TenantId;
            builder.UserName = UserName;
            builder.Email = UserName;
            builder.Add(PermissionEntityType.Project, PermissionLevel.Write);
        });

        var project = new TestProject { Name = "Test Project", Id = 1, TenantId = TenantId, Slug = "test-project", ShortName = "TP" };
        await fakeRepository.AddAsync(project);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await projectManager.DeleteAsync(principal, project));
    }

    /// <summary>
    /// Tests that ProjectManager.AddAsync calls IMediator.Publish when a project is created.
    /// </summary>
    [Fact]
    public async Task AddAsync_ShouldCallMediatorPublish_WhenProjectIsCreated()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var memoryCache = Substitute.For<IMemoryCache>();
        var fakeRepository = new FakeProjectRepository();
        var projectManager = new ProjectManager(fakeRepository, memoryCache, mediator, NullLogger<ProjectManager>.Instance);

        var principal = Impersonation.Impersonate(TenantId);
        var project = new TestProject { Name = "Test Project", Slug = "test-project", ShortName = "TP" };

        // Act
        await projectManager.AddAsync(principal, project);

        // Assert
        await mediator.Received(1).Publish(Arg.Is<ProjectCreated>(e => e.Project == project), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that ProjectManager.UpdateProjectAsync calls IMediator.Publish when a project is updated.
    /// </summary>
    [Fact]
    public async Task UpdateProjectAsync_ShouldCallMediatorPublish_WhenProjectIsUpdated()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var memoryCache = Substitute.For<IMemoryCache>();
        var fakeRepository = new FakeProjectRepository();
        var projectManager = new ProjectManager(fakeRepository, memoryCache, mediator, NullLogger<ProjectManager>.Instance);

        var principal = Impersonation.Impersonate(TenantId);
        var project = new TestProject { Name = "Updated Project", Slug = "updated-project", ShortName = "UP", TenantId = TenantId };

        // Act
        await projectManager.UpdateProjectAsync(principal, project);

        // Assert
        await mediator.Received(1).Publish(Arg.Is<ProjectUpdated>(e => e.Project == project), Arg.Any<CancellationToken>());
    }
}
