using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Threading;
using Xunit;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Issues.Models;
using TestBucket.Contracts.Issues.Models;
using TestBucket.Domain.Milestones;
using TestBucket.Domain.Milestones.DataSources;
using TestBucket.Contracts.Fields;

namespace TestBucket.Domain.UnitTests.Milestones;
/// <summary>
/// Unit tests for <see cref="MilestoneDataSource"/> and milestone-related data source logic.
/// </summary>
[EnrichedTest]
[UnitTest]
[Component("Milestones")]
[FunctionalTest]
public class MilestoneDataSourceTests
{
    /// <summary>
    /// Creates a test instance of <see cref="MilestoneDataSource"/> and <see cref="IMilestoneManager"/>.
    /// </summary>
    /// <returns>Tuple containing the data source and manager.</returns>
    private static (MilestoneDataSource, IMilestoneManager) CreateSut() 
    {
        var manager = new MilestoneManager(new FakeMilestoneRepository());
        var dataSource = new MilestoneDataSource(manager);
        return (dataSource, manager);
    }

    private const int TestProjectId =1;
    private const string TenantId = "tenant-1";

    /// <summary>
    /// Creates a <see cref="ClaimsPrincipal"/> with all permissions for testing.
    /// </summary>
    /// <returns>A principal with all permissions.</returns>
    private ClaimsPrincipal CreateUserWithAllPermissions() => Impersonation.Impersonate(builder =>
    {
        builder.UserName = "user";
        builder.TenantId = TenantId;
        builder.AddAllPermissions();
    });

    /// <summary>
    /// Verifies <see cref="MilestoneDataSource.GetOptionsAsync"/> returns milestones with titles when type is Milestones.
    /// </summary>
    [Fact]
    public async Task GetOptionsAsync_ReturnsMilestonesWithTitles_WhenTypeIsMilestones()
    {
        var (dataSource, manager) = CreateSut();
        var principal = CreateUserWithAllPermissions();
        var milestone1 = new Milestone { Title = "Alpha", Description = "First", TestProjectId = TestProjectId, TenantId = TenantId };
        var milestone2 = new Milestone { Title = "Beta", Description = "Second", TestProjectId = TestProjectId, TenantId = TenantId };
        var milestone3 = new Milestone { Title = null, Description = "NoTitle", TestProjectId = TestProjectId, TenantId = TenantId };
        await manager.AddMilestoneAsync(principal, milestone1);
        await manager.AddMilestoneAsync(principal, milestone2);
        await manager.AddMilestoneAsync(principal, milestone3);

        var result = await dataSource.GetOptionsAsync(principal, FieldDataSourceType.Milestones, TestProjectId, CancellationToken.None);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, x => x.Title == "Alpha" && x.Description == "First");
        Assert.Contains(result, x => x.Title == "Beta" && x.Description == "Second");
        Assert.DoesNotContain(result, x => x.Description == "NoTitle");
    }

    /// <summary>
    /// Verifies <see cref="MilestoneDataSource.GetOptionsAsync"/> returns empty when type is not Milestones.
    /// </summary>
    [Fact]
    public async Task GetOptionsAsync_ReturnsEmpty_WhenTypeIsNotMilestones()
    {
        var (dataSource, manager) = CreateSut();
        var principal = CreateUserWithAllPermissions();
        var milestone = new Milestone { Title = "Alpha", Description = "First", TestProjectId = TestProjectId, TenantId = TenantId };
        await manager.AddMilestoneAsync(principal, milestone);
        var result = await dataSource.GetOptionsAsync(principal, FieldDataSourceType.Features, TestProjectId, CancellationToken.None);
        Assert.Empty(result);
    }

    /// <summary>
    /// Verifies <see cref="MilestoneDataSource.SearchOptionsAsync"/> returns matching milestones when type is Milestones.
    /// </summary>
    [Fact]
    public async Task SearchOptionsAsync_ReturnsMatchingMilestones_WhenTypeIsMilestones()
    {
        var (dataSource, manager) = CreateSut();
        var principal = CreateUserWithAllPermissions();
        var milestone1 = new Milestone { Title = "AlphaX", Description = "First", Color = "Red", TestProjectId = TestProjectId, TenantId = TenantId };
        var milestone2 = new Milestone { Title = "AlphaY", Description = "Second", Color = "Blue", TestProjectId = TestProjectId, TenantId = TenantId };
        var milestone3 = new Milestone { Title = "Gamma", Description = "Third", Color = "Green", TestProjectId = TestProjectId, TenantId = TenantId };
        await manager.AddMilestoneAsync(principal, milestone1);
        await manager.AddMilestoneAsync(principal, milestone2);
        await manager.AddMilestoneAsync(principal, milestone3);

        var result = await dataSource.SearchOptionsAsync(principal, FieldDataSourceType.Milestones, TestProjectId, "Alpha", 10, CancellationToken.None);
        Assert.Contains(result, x => x.Title == "AlphaX" && x.Color == "Red");
        Assert.Contains(result, x => x.Title == "AlphaY" && x.Color == "Blue");
        Assert.DoesNotContain(result, x => x.Title == "Gamma");
        Assert.DoesNotContain(result, x => x.Color == "Green");
    }

    /// <summary>
    /// Verifies <see cref="MilestoneDataSource.SearchOptionsAsync"/> returns empty when type is not Milestones.
    /// </summary>
    [Fact]
    public async Task SearchOptionsAsync_ReturnsEmpty_WhenTypeIsNotMilestones()
    {
        var (dataSource, manager) = CreateSut();
        var principal = CreateUserWithAllPermissions();
        var milestone = new Milestone { Title = "Alpha", Description = "First", TestProjectId = TestProjectId, TenantId = TenantId };
        await manager.AddMilestoneAsync(principal, milestone);
        var result = await dataSource.SearchOptionsAsync(principal, FieldDataSourceType.Features, TestProjectId, "Alpha", 10, CancellationToken.None);
        Assert.Empty(result);
    }

}
