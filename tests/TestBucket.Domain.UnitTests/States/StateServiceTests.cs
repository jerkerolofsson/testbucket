using System.Security.Claims;

using Microsoft.Extensions.DependencyInjection;

using NSubstitute;

using TestBucket.Contracts.Issues.States;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Projects.Models;
using TestBucket.Domain.States;
using TestBucket.Domain.States.Caching;
using TestBucket.Domain.States.Models;

namespace TestBucket.Domain.UnitTests.States;

/// <summary>
/// Unit tests for the StateService functionality including state retrieval, merging, 
/// permissions validation, and cache invalidation behavior.
/// </summary>
[UnitTest]
[Component("States & Types")]
[EnrichedTest]
[FunctionalTest]
public class StateServiceTests
{
    private const long ProjectId = 1;
    private const long TeamId = 2;
    private const string TenantId = "tenant-1";
    private static readonly TestProject _project = new TestProject
    {
        ShortName = "TP",
        Slug = "test-project",
        Id = ProjectId,
        Name = "Test Project",
        TeamId = TeamId,
        TenantId = TenantId
    };

    /// <summary>
    /// Creates a ClaimsPrincipal with all permissions for testing purposes.
    /// </summary>
    /// <returns>A ClaimsPrincipal with full permissions for the test tenant.</returns>
    internal ClaimsPrincipal GetUserWithAllPermissions()
    {
        return Impersonation.Impersonate(TenantId);
    }

    /// <summary>
    /// Creates a ClaimsPrincipal with limited permissions for security testing.
    /// </summary>
    /// <param name="level">The permission level to assign to the user.</param>
    /// <returns>A ClaimsPrincipal with the specified permission level.</returns>
    internal ClaimsPrincipal GetUserWithLimitedPermissions(PermissionLevel level)
    {
        return Impersonation.Impersonate(x =>
        {
            x.Email = x.UserName = "state@internet.com";
            x.TenantId = TenantId;
            x.Add(PermissionEntityType.Project, level);
        });
    }

    /// <summary>
    /// Creates a system under test (SUT) instance of IStateService with all required dependencies.
    /// </summary>
    /// <returns>A configured IStateService instance for testing.</returns>
    internal IStateService CreateSut()
    {
        var projectRepo = Substitute.For<IProjectRepository>();
        projectRepo.GetProjectByIdAsync(TenantId, ProjectId).Returns(_project);

        var services = new ServiceCollection();
        services.AddSingleton<IProjectRepository>(projectRepo);
        services.AddScoped<IStateRepository, Fakes.FakeStateRepository>();
        services.AddMediator(options => options.ServiceLifetime = ServiceLifetime.Scoped);
        services.AddLogging();
        services.AddScoped<IStateService, StateService>();
        services.AddSingleton<ProjectStateCache>();
        return services.BuildServiceProvider().GetRequiredService<IStateService>();
    }

    /// <summary>
    /// Tests that GetIssueStatesAsync returns default states when no custom state definitions exist.
    /// This ensures the system provides fallback behavior when no customizations are configured.
    /// </summary>
    [FunctionalTest]
    [Fact]
    public async Task GetIssueStatesAsync_ReturnsDefaultStates_WhenNoCustomStatesExist()
    {
        // Arrange
        var sut = CreateSut();
        var user = GetUserWithAllPermissions();

        // Act
        var states = await sut.GetIssueStatesAsync(user, ProjectId);

        // Assert
        Assert.NotNull(states);
        Assert.Equal(DefaultStates.GetDefaultIssueStates().Count, states.Count);
    }

    /// <summary>
    /// Tests that GetRequirementStatesAsync throws UnauthorizedAccessException when the user 
    /// lacks sufficient permissions. This validates the security boundary for state access.
    /// </summary>
    [SecurityTest]
    [Fact]
    public async Task GetRequirementStatesAsync_ThrowsUnauthorizedAccessException_WhenUserLacksPermission()
    {
        // Arrange
        var sut = CreateSut();
        var user = GetUserWithLimitedPermissions(PermissionLevel.None);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
        {
            await sut.GetRequirementStatesAsync(user, ProjectId);
        });
    }

    /// <summary>
    /// Tests that DeleteDefinitionAsync throws UnauthorizedAccessException when the user 
    /// has insufficient permissions (ReadWrite instead of required Delete permission).
    /// This validates that deletion operations require appropriate authorization levels.
    /// </summary>
    [SecurityTest]
    [Fact]
    public async Task DeleteDefinitionAsync_ThrowsUnauthorizedAccessException_WhenUserLacksPermission()
    {
        // Arrange
        var sut = CreateSut();
        var user = GetUserWithLimitedPermissions(PermissionLevel.ReadWrite);
        var state = await sut.GetTenantDefinitionAsync(user);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
        {
            await sut.DeleteDefinitionAsync(user, state);
        });
    }

    /// <summary>
    /// Tests that SaveDefinitionAsync properly invalidates the cache when saving project-level 
    /// state definitions. This ensures that cached data is refreshed after modifications.
    /// </summary>
    [FunctionalTest]
    [Fact]
    public async Task SaveDefinitionAsync_WithProjectDefinition_InvalidatesCache()
    {
        // Arrange
        var sut = CreateSut();
        var user = GetUserWithAllPermissions();

        // Trigger cache population
        await sut.GetProjectDefinitionAsync(user, ProjectId);

        var stateDefinition = new StateDefinition
        {
            Id = 1,
            TestProjectId = ProjectId,
            IssueStates = [],
            RequirementStates = DefaultStates.GetDefaultRequirementStates().ToList(),
            TestCaseStates = DefaultStates.GetDefaultTestCaseStates().ToList(),
            TestCaseRunStates = DefaultStates.GetDefaultTestCaseRunStates().ToList()
        };

        // Act
        await sut.SaveDefinitionAsync(user, stateDefinition);

        // Assert
        var definition = await sut.GetProjectDefinitionAsync(user, ProjectId);
        Assert.Empty(definition.IssueStates);
    }

    /// <summary>
    /// Tests that GetIssueStatesAsync correctly merges tenant and team level state definitions 
    /// after a project definition has been deleted. This validates the hierarchical merging 
    /// behavior and fallback to higher-level definitions when project-specific ones are removed.
    /// </summary>
    [FunctionalTest]
    [Fact]
    public async Task GetIssueStatesAsync_WithTenantAndTeam_AfterDeleteProject_MergedCorrectly()
    {
        // Arrange
        var sut = CreateSut();
        var user = GetUserWithAllPermissions();

        // Tenant def
        var tenantDefinition = await sut.GetTenantDefinitionAsync(user);
        tenantDefinition.Clear();
        tenantDefinition.IssueStates = [new IssueState("Open", MappedIssueState.Open)];
        await sut.SaveDefinitionAsync(user, tenantDefinition);

        // Team def
        var teamDefinition = await sut.GetTeamDefinitionAsync(user, TeamId);
        teamDefinition.Clear();
        teamDefinition.IssueStates = [new IssueState("Closed", MappedIssueState.Closed)];
        await sut.SaveDefinitionAsync(user, teamDefinition);

        // Project def
        var projectDef = await sut.GetProjectDefinitionAsync(user, ProjectId);
        projectDef.Clear();
        projectDef.IssueStates = [new IssueState("Triage", MappedIssueState.Triage)];
       
        await sut.SaveDefinitionAsync(user, projectDef);
        await sut.DeleteDefinitionAsync(user, projectDef);

        // Act
        var definition = await sut.GetIssueStatesAsync(user, ProjectId);

        // Assert
        Assert.Equal(2, definition.Count);
        Assert.Contains(definition, x => x.MappedState == MappedIssueState.Open);
        Assert.Contains(definition, x => x.MappedState == MappedIssueState.Closed);
    }

    /// <summary>
    /// Tests that GetIssueStatesAsync correctly merges state definitions from all three levels: 
    /// tenant, team, and project. This validates the complete hierarchical merging behavior 
    /// where project-level definitions take precedence and all levels are properly combined.
    /// </summary>
    [FunctionalTest]
    [Fact]
    public async Task GetIssueStatesAsync_WithTenantTeamAndProjectDefinitions_MergedCorrectly()
    {
        // Arrange
        var sut = CreateSut();
        var user = GetUserWithAllPermissions();

        // Tenant def
        var tenantDefinition = await sut.GetTenantDefinitionAsync(user);
        tenantDefinition.Clear();
        tenantDefinition.IssueStates = [new IssueState("Open", MappedIssueState.Open)];
        await sut.SaveDefinitionAsync(user, tenantDefinition);

        // Team def
        var teamDefinition = await sut.GetTeamDefinitionAsync(user, TeamId);
        teamDefinition.Clear();
        teamDefinition.IssueStates = [new IssueState("Closed", MappedIssueState.Closed)];
        await sut.SaveDefinitionAsync(user, teamDefinition);

        // Project def
        var projectDef = await sut.GetProjectDefinitionAsync(user, ProjectId);
        projectDef.Clear();
        projectDef.IssueStates = [new IssueState("Triage", MappedIssueState.Triage)];

        // Act
        var definition = await sut.GetIssueStatesAsync(user, ProjectId);

        // Assert
        Assert.Equal(3, definition.Count);
        Assert.Contains(definition, x => x.MappedState == MappedIssueState.Open);
        Assert.Contains(definition, x => x.MappedState == MappedIssueState.Closed);
        Assert.Contains(definition, x => x.MappedState == MappedIssueState.Triage);
    }

    /// <summary>
    /// Tests that GetIssueStatesAsync correctly merges tenant and team level state definitions 
    /// when no project-specific definition exists. This validates the hierarchical merging 
    /// behavior for the two-level scenario.
    /// </summary>
    [FunctionalTest]
    [Fact]
    public async Task GetIssueStatesAsync_WithTenantAndTeamDefinitions_MergedCorrectly()
    {
        // Arrange
        var sut = CreateSut();
        var user = GetUserWithAllPermissions();

        // Tenant def
        var tenantDefinition = await sut.GetTenantDefinitionAsync(user);
        tenantDefinition.Clear();
        tenantDefinition.IssueStates = [new IssueState("Open", MappedIssueState.Open)];
        await sut.SaveDefinitionAsync(user, tenantDefinition);

        // Team def
        var teamDefinition = await sut.GetTeamDefinitionAsync(user, TeamId);
        teamDefinition.Clear();
        teamDefinition.IssueStates = [new IssueState("Closed", MappedIssueState.Closed)];
        await sut.SaveDefinitionAsync(user, teamDefinition);

        // Act
        var definition = await sut.GetIssueStatesAsync(user, ProjectId);

        // Assert
        Assert.Equal(2, definition.Count);
        Assert.Contains(definition, x => x.MappedState == MappedIssueState.Open);
        Assert.Contains(definition, x => x.MappedState == MappedIssueState.Closed);
    }

    /// <summary>
    /// Tests that tenant state definitions for different tenants do not cross-contaminate.
    /// </summary>
    [SecurityTest]
    [Fact]
    public async Task StateDefinitions_AreIsolated_BetweenTenants()
    {
        // Arrange
        const string tenant1 = "tenant-1";
        const string tenant2 = "tenant-2";
        const long project1 = 1;
        const long project2 = 2;
        const long team1 = 10;
        const long team2 = 20;

        // Setup SUT for tenant 1
        var projectRepo1 = Substitute.For<IProjectRepository>();
        projectRepo1.GetProjectByIdAsync(tenant1, project1).Returns(new TestProject
        {
            ShortName = "TP1",
            Slug = "test-project-1",
            Id = project1,
            Name = "Test Project 1",
            TeamId = team1,
            TenantId = tenant1
        });
        projectRepo1.GetProjectByIdAsync(tenant2, project2).Returns(new TestProject
        {
            ShortName = "TP2",
            Slug = "test-project-2",
            Id = project2,
            Name = "Test Project 2",
            TeamId = team2,
            TenantId = tenant2
        });
        var services1 = new ServiceCollection();
        services1.AddSingleton<IProjectRepository>(projectRepo1);
        services1.AddScoped<IStateRepository, Fakes.FakeStateRepository>();
        services1.AddMediator(options => options.ServiceLifetime = ServiceLifetime.Scoped);
        services1.AddLogging();
        services1.AddScoped<IStateService, StateService>();
        services1.AddSingleton<ProjectStateCache>();
        var sut1 = services1.BuildServiceProvider().GetRequiredService<IStateService>();
        var user1 = Impersonation.Impersonate(tenant1);
        var user2 = Impersonation.Impersonate(tenant2);

        // Tenant 1: set unique state
        var tenant1Def = await sut1.GetTenantDefinitionAsync(user1);
        tenant1Def.Clear();
        tenant1Def.IssueStates = [new IssueState("Tenant1-Open", MappedIssueState.Open)];
        await sut1.SaveDefinitionAsync(user1, tenant1Def);

        // Tenant 2: set unique state
        var tenant2Def = await sut1.GetTenantDefinitionAsync(user2);
        tenant2Def.Clear();
        tenant2Def.IssueStates = [new IssueState("Tenant2-Closed", MappedIssueState.Closed)];
        await sut1.SaveDefinitionAsync(user2, tenant2Def);

        // Act
        var tenant1States = await sut1.GetIssueStatesAsync(user1, project1);
        var tenant2States = await sut1.GetIssueStatesAsync(user2, project2);

        // Assert
        Assert.Single(tenant1States);
        Assert.Equal("Tenant1-Open", tenant1States[0].Name);
        Assert.DoesNotContain(tenant1States, x => x.Name == "Tenant2-Closed");

        Assert.Single(tenant2States);
        Assert.Equal("Tenant2-Closed", tenant2States[0].Name);
        Assert.DoesNotContain(tenant2States, x => x.Name == "Tenant1-Open");
    }

    /// <summary>
    /// Tests that GetTestCaseStatesAsync returns default test case states when no custom state definitions exist.
    /// </summary>
    [FunctionalTest]
    [Fact]
    public async Task GetTestCaseStatesAsync_ReturnsDefaultStates_WhenNoCustomStatesExist()
    {
        var sut = CreateSut();
        var user = GetUserWithAllPermissions();
        var states = await sut.GetTestCaseStatesAsync(user, ProjectId);
        Assert.NotNull(states);
        Assert.Equal(DefaultStates.GetDefaultTestCaseStates().Count, states.Count);
    }

    /// <summary>
    /// Tests that GetTestCaseRunStatesAsync returns default test case run states when no custom state definitions exist.
    /// </summary>
    [FunctionalTest]
    [Fact]
    public async Task GetTestCaseRunStatesAsync_ReturnsDefaultStates_WhenNoCustomStatesExist()
    {
        var sut = CreateSut();
        var user = GetUserWithAllPermissions();
        var states = await sut.GetTestCaseRunStatesAsync(user, ProjectId);
        Assert.NotNull(states);
        Assert.Equal(DefaultStates.GetDefaultTestCaseRunStates().Count, states.Count);
    }

    /// <summary>
    /// Tests that GetTestCaseRunFinalStateAsync returns the expected final state.
    /// </summary>
    [FunctionalTest]
    [Fact]
    public async Task GetTestCaseRunFinalStateAsync_ReturnsFinalState()
    {
        var sut = CreateSut();
        var user = GetUserWithAllPermissions();
        var state = await sut.GetTestCaseRunFinalStateAsync(user, ProjectId);
        Assert.NotNull(state);
        Assert.Equal(DefaultStates.GetDefaultTestCaseRunFinalState().Name, state.Name);
    }

    /// <summary>
    /// Tests that GetTestCaseRunInitialStateAsync returns the expected initial state.
    /// </summary>
    [FunctionalTest]
    [Fact]
    public async Task GetTestCaseRunInitialStateAsync_ReturnsInitialState()
    {
        var sut = CreateSut();
        var user = GetUserWithAllPermissions();
        var state = await sut.GetTestCaseRunInitialStateAsync(user, ProjectId);
        Assert.NotNull(state);
        Assert.Equal(DefaultStates.GetDefaultTestCaseRunInitialState().Name, state.Name);
    }

    /// <summary>
    /// Tests that GetRequirementTypesAsync returns non-empty types.
    /// </summary>
    [FunctionalTest]
    [Fact]
    public async Task GetRequirementTypesAsync_ReturnsTypes()
    {
        var sut = CreateSut();
        var user = GetUserWithAllPermissions();
        var types = await sut.GetRequirementTypesAsync(user, ProjectId);
        Assert.NotNull(types);
        Assert.NotEmpty(types);
    }

    /// <summary>
    /// Tests that GetIssueTypesAsync returns non-empty types.
    /// </summary>
    [FunctionalTest]
    [Fact]
    public async Task GetIssueTypesAsync_ReturnsTypes()
    {
        var sut = CreateSut();
        var user = GetUserWithAllPermissions();
        var types = await sut.GetIssueTypesAsync(user, ProjectId);
        Assert.NotNull(types);
        Assert.NotEmpty(types);
    }
}
