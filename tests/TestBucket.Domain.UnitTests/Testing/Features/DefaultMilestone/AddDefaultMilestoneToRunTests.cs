using System.Security.Claims;

using Mediator;

using Microsoft.Extensions.Caching.Memory;

using Quartz;

using TestBucket.Contracts.Fields;
using TestBucket.Contracts.Issues.Models;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Milestones;
using TestBucket.Domain.Testing.Features.DefaultMilestones;
using TestBucket.Domain.UnitTests.Fakes;
using TestBucket.Domain.UnitTests.Milestones;
using TestBucket.Traits.Core;

namespace TestBucket.Domain.UnitTests.Testing.Features.DefaultMilestone;

/// <summary>
/// Contains unit tests for verifying the behavior of adding default milestones to test runs.
/// </summary>
[EnrichedTest]
[UnitTest]
[FunctionalTest]
[Component("Testing")]
[Feature("Default Milestone")]
public class AddDefaultMilestoneToRunTests
{
    // Mocks
    private static readonly ISchedulerFactory _schedulerFactory = NSubstitute.Substitute.For<ISchedulerFactory>();
    private static readonly IMemoryCache _memoryCache = NSubstitute.Substitute.For<IMemoryCache>();
    private static readonly IMediator _mediator = NSubstitute.Substitute.For<IMediator>();

    // Fakes
    private static readonly TimeProvider _timeProvider = new FakeTimeProvider(new DateTimeOffset(2025, 09, 22, 1, 2, 3, TimeSpan.Zero));

    /// <summary>
    /// Creates a new instance of <see cref="IMilestoneManager"/> using a fake repository and time provider.
    /// </summary>
    /// <returns>A new <see cref="IMilestoneManager"/> instance.</returns>
    private static IMilestoneManager CreateMilestoneManager() => new MilestoneManager(new FakeMilestoneRepository(), _timeProvider);

    /// <summary>
    /// Creates a new instance of <see cref="IFieldManager"/> using a fake repository, mediator, memory cache, and scheduler factory.
    /// </summary>
    /// <returns>A new <see cref="IFieldManager"/> instance.</returns>
    private static IFieldManager CreateFieldManager() => new FieldManager(new FakeFieldRepository(), _mediator, _memoryCache, _schedulerFactory);

    // Test constants
    private const int ProjectId = 1;
    private const string TenantId = "tenant-1";

    /// <summary>
    /// Verifies that a milestone field is added to the test run when:
    /// 1. An open milestone exists.
    /// 2. The milestone start date is before the current date.
    /// 3. The milestone end date is after the current date.
    /// 4. A milestone field exists.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task AddAsync_AddsDefaultMilestoneField_WhenNoMilestoneFieldExists()
    {
        // Arrange
        var principal = Impersonation.Impersonate(builder =>
        {
            builder.UserName = "user";
            builder.TenantId = TenantId;
            builder.AddAllPermissions();
        });

        var milestoneManager = CreateMilestoneManager();
        var fieldManager = CreateFieldManager();

        // Add a current milestone to the project
        var milestone = new Milestone
        {
            TenantId = TenantId,
            TestProjectId = ProjectId,
            Title = "Release 1.0",
            State = MilestoneState.Open,
            StartDate = _timeProvider.GetUtcNow().AddDays(-1),
            EndDate = _timeProvider.GetUtcNow().AddDays(1)
        };
        await milestoneManager.AddMilestoneAsync(principal, milestone);

        // Create a milestone field definition
        var milestoneFieldDefinition = new FieldDefinition
        {
            Id = 100,
            Name = "Milestone",
            TraitType = TraitType.Milestone,
            Type = FieldType.String,
            Target = FieldTarget.TestRun,
            TenantId = TenantId
        };
        var fieldDefinitions = new List<FieldDefinition> { milestoneFieldDefinition };

        // Create a test run
        var testRun = new TestRun
        {
            Id = 123,
            Name = "Test Run 1",
            TenantId = TenantId,
            TestProjectId = ProjectId
        };

        // Act
        await AddDefaultMilestoneToRun.AddAsync(principal, testRun, fieldDefinitions, milestoneManager, fieldManager);

        // Assert
        var fields = await fieldManager.GetTestRunFieldsAsync(principal, testRun.Id, fieldDefinitions);
        var milestoneField = fields.FirstOrDefault(f => f.FieldDefinitionId == milestoneFieldDefinition.Id);
        Assert.NotNull(milestoneField);
        Assert.Equal("Release 1.0", milestoneField.StringValue);
    }

    /// <summary>
    /// Creates a <see cref="ClaimsPrincipal"/> for the specified tenant.
    /// </summary>
    /// <param name="tenantId">The tenant ID. Defaults to <see cref="TenantId"/>.</param>
    /// <returns>A <see cref="ClaimsPrincipal"/> instance.</returns>
    private static ClaimsPrincipal CreatePrincipal(string tenantId = TenantId)
    {
        return Impersonation.Impersonate(builder =>
        {
            builder.UserName = "user";
            builder.TenantId = tenantId;
            builder.AddAllPermissions();
        });
    }

    /// <summary>
    /// Creates a milestone <see cref="FieldDefinition"/> for use in tests.
    /// </summary>
    /// <returns>A <see cref="FieldDefinition"/> representing a milestone field.</returns>
    private static FieldDefinition CreateMilestoneFieldDefinition()
    {
        return new FieldDefinition
        {
            Id = 100,
            Name = "Milestone",
            TraitType = TraitType.Milestone,
            Type = FieldType.String,
            Target = FieldTarget.TestRun
        };
    }

    /// <summary>
    /// Creates a <see cref="TestRun"/> instance for use in tests.
    /// </summary>
    /// <param name="projectId">The project ID. Defaults to <see cref="ProjectId"/>.</param>
    /// <param name="tenantId">The tenant ID. Defaults to <see cref="TenantId"/>.</param>
    /// <returns>A <see cref="TestRun"/> instance.</returns>
    private static TestRun CreateTestRun(long? projectId = ProjectId, string? tenantId = TenantId)
    {
        return new TestRun
        {
            Id = 123,
            Name = "Test Run 1",
            TenantId = tenantId,
            TestProjectId = projectId
        };
    }

    /// <summary>
    /// Verifies that a milestone field is not added when the milestone is closed.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task NotAdded_WhenMilestoneIsClosed()
    {
        var principal = CreatePrincipal();
        var milestoneManager = CreateMilestoneManager();
        var fieldManager = CreateFieldManager();

        var milestone = new Milestone
        {
            TenantId = TenantId,
            TestProjectId = ProjectId,
            Title = "Closed Milestone",
            State = MilestoneState.Closed,
            StartDate = _timeProvider.GetUtcNow().AddDays(-1),
            EndDate = _timeProvider.GetUtcNow().AddDays(1)
        };
        await milestoneManager.AddMilestoneAsync(principal, milestone);

        var fieldDefinitions = new[] { CreateMilestoneFieldDefinition() };
        var testRun = CreateTestRun();

        await AddDefaultMilestoneToRun.AddAsync(principal, testRun, fieldDefinitions, milestoneManager, fieldManager);

        var fields = await fieldManager.GetTestRunFieldsAsync(principal, testRun.Id, fieldDefinitions);
        Assert.Null(fields.Where(x => x.FieldDefinitionId == 100).Select(x => x.StringValue).FirstOrDefault());
    }

    /// <summary>
    /// Verifies that a milestone field is not added when the milestone's end and start dates are before the current date.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task NotAdded_WhenMilestoneEndAndStartDateBeforeNow()
    {
        var principal = CreatePrincipal();
        var milestoneManager = CreateMilestoneManager();
        var fieldManager = CreateFieldManager();

        var milestone = new Milestone
        {
            TenantId = TenantId,
            TestProjectId = ProjectId,
            Title = "Expired Milestone",
            State = MilestoneState.Open,
            StartDate = _timeProvider.GetUtcNow().AddDays(-10),
            EndDate = _timeProvider.GetUtcNow().AddDays(-5)
        };
        await milestoneManager.AddMilestoneAsync(principal, milestone);

        var fieldDefinitions = new[] { CreateMilestoneFieldDefinition() };
        var testRun = CreateTestRun();

        await AddDefaultMilestoneToRun.AddAsync(principal, testRun, fieldDefinitions, milestoneManager, fieldManager);

        var fields = await fieldManager.GetTestRunFieldsAsync(principal, testRun.Id, fieldDefinitions);
        Assert.Null(fields.Where(x => x.FieldDefinitionId == 100).Select(x => x.StringValue).FirstOrDefault());
    }

    /// <summary>
    /// Verifies that a milestone field is not added when the project ID is different.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task NotAdded_WhenProjectIdIsDifferent()
    {
        var principal = CreatePrincipal();
        var milestoneManager = CreateMilestoneManager();
        var fieldManager = CreateFieldManager();

        var milestone = new Milestone
        {
            TenantId = TenantId,
            TestProjectId = ProjectId + 1, // Different project
            Title = "Other Project Milestone",
            State = MilestoneState.Open,
            StartDate = _timeProvider.GetUtcNow().AddDays(-1),
            EndDate = _timeProvider.GetUtcNow().AddDays(1)
        };
        await milestoneManager.AddMilestoneAsync(principal, milestone);

        var fieldDefinitions = new[] { CreateMilestoneFieldDefinition() };
        var testRun = CreateTestRun();

        await AddDefaultMilestoneToRun.AddAsync(principal, testRun, fieldDefinitions, milestoneManager, fieldManager);

        var fields = await fieldManager.GetTestRunFieldsAsync(principal, testRun.Id, fieldDefinitions);
        Assert.Null(fields.Where(x => x.FieldDefinitionId == 100).Select(x => x.StringValue).FirstOrDefault());
    }

    /// <summary>
    /// Verifies that a milestone field is not added when the test run's project ID is null.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task NotAdded_WhenTestProjectIdIsNull()
    {
        var principal = CreatePrincipal();
        var milestoneManager = CreateMilestoneManager();
        var fieldManager = CreateFieldManager();

        var fieldDefinitions = new[] { CreateMilestoneFieldDefinition() };
        var testRun = CreateTestRun(projectId: null);

        await AddDefaultMilestoneToRun.AddAsync(principal, testRun, fieldDefinitions, milestoneManager, fieldManager);

        var fields = await fieldManager.GetTestRunFieldsAsync(principal, testRun.Id, fieldDefinitions);
        Assert.Null(fields.Where(x => x.FieldDefinitionId == 100).Select(x => x.StringValue).FirstOrDefault());
    }

    /// <summary>
    /// Verifies that a milestone field is not added when the test run's tenant ID is null.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task NotAdded_WhenTenantIdIsNull()
    {
        var principal = CreatePrincipal();
        var milestoneManager = CreateMilestoneManager();
        var fieldManager = CreateFieldManager();

        var fieldDefinitions = new[] { CreateMilestoneFieldDefinition() };
        var testRun = CreateTestRun(tenantId: null);

        await AddDefaultMilestoneToRun.AddAsync(principal, testRun, fieldDefinitions, milestoneManager, fieldManager);

        var fields = await fieldManager.GetTestRunFieldsAsync(principal, testRun.Id, fieldDefinitions);
        Assert.Null(fields.Where(x => x.FieldDefinitionId == 100).Select(x => x.StringValue).FirstOrDefault());
    }

    /// <summary>
    /// Verifies that a milestone field is not added when the milestone's tenant ID is different from the test run's tenant ID.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task NotAdded_WhenMilestoneTenantIdIsDifferent()
    {
        var milestoneManager = CreateMilestoneManager();
        var fieldManager = CreateFieldManager();

        var milestonePrincipal = CreatePrincipal("other-tenant");
        var milestone = new Milestone
        {
            TestProjectId = ProjectId,
            Title = "Other Tenant Milestone",
            State = MilestoneState.Open,
            StartDate = _timeProvider.GetUtcNow().AddDays(-1),
            EndDate = _timeProvider.GetUtcNow().AddDays(1)
        };
        await milestoneManager.AddMilestoneAsync(milestonePrincipal, milestone);

        var fieldDefinitions = new[] { CreateMilestoneFieldDefinition() };
        var testRun = CreateTestRun();

        // Act
        var principal = CreatePrincipal();
        await AddDefaultMilestoneToRun.AddAsync(principal, testRun, fieldDefinitions, milestoneManager, fieldManager);

        // Assert
        var fields = await fieldManager.GetTestRunFieldsAsync(principal, testRun.Id, fieldDefinitions);
        Assert.Null(fields.Where(x => x.FieldDefinitionId == 100).Select(x => x.StringValue).FirstOrDefault());
    }

}