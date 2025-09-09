using System.Security.Claims;

using Microsoft.Extensions.DependencyInjection;

using TestBucket.Domain.Fields;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.UnitTests.Fakes;

namespace TestBucket.Domain.UnitTests.Fields;

/// <summary>
/// Unit tests for verifying write permissions in the FieldManager.
/// </summary>
[Feature("Custom Fields")]
[UnitTest]
[Component("Fields")]
[EnrichedTest]
[SecurityTest]
public class FieldManagerWritePermissionTests
{
    private readonly IFieldManager _fieldManager;
    private const string TenantId = "tenant-1";
    private const string UserName = "user@internet.com";

    /// <summary>
    /// Initializes a new instance of the <see cref="FieldManagerWritePermissionTests"/> class.
    /// Sets up the required services and dependencies for testing.
    /// </summary>
    public FieldManagerWritePermissionTests()
    {
        var services = new ServiceCollection();
        services.AddMemoryCache();
        services.AddLogging();
        services.AddScoped<IFieldRepository, FakeFieldRepository>();
        services.AddScoped<IFieldManager, FieldManager>();
        services.AddScoped<IFieldDefinitionManager, FieldDefinitionManager>();
        services.AddMediator(options => options.ServiceLifetime = ServiceLifetime.Scoped);
        _fieldManager = services.BuildServiceProvider().GetRequiredService<IFieldManager>();
    }

    /// <summary>
    /// Tests that attempting to upsert a requirement field with only read permissions throws an <see cref="UnauthorizedAccessException"/>.
    /// </summary>
    [Fact]
    public async Task UpsertRequirementFieldAsync_WithOnlyReadPermission_ThrowsUnauthorizedException()
    {
        ClaimsPrincipal principal = GetUserWithPermissions(PermissionEntityType.Requirement, PermissionLevel.Read);
        var field = new RequirementField { FieldDefinitionId = 1, RequirementId = 1, StringValue = "Test" };
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _fieldManager.UpsertRequirementFieldAsync(principal, field));
    }

    /// <summary>
    /// Tests that attempting to upsert a test case run field with only read permissions throws an <see cref="UnauthorizedAccessException"/>.
    /// </summary>
    [Fact]
    public async Task UpsertTestCaseRunFieldAsync_WithOnlyReadPermission_ThrowsUnauthorizedException()
    {
        ClaimsPrincipal principal = GetUserWithPermissions(PermissionEntityType.TestCaseRun, PermissionLevel.Read);
        var field = new TestCaseRunField { FieldDefinitionId = 1, TestCaseRunId = 1, TestRunId = 1, StringValue = "Test" };
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _fieldManager.UpsertTestCaseRunFieldAsync(principal, field));
    }

    /// <summary>
    /// Tests that attempting to upsert a test case field with only read permissions throws an <see cref="UnauthorizedAccessException"/>.
    /// </summary>
    [Fact]
    public async Task UpsertTestCaseFieldAsync_WithOnlyReadPermission_ThrowsUnauthorizedException()
    {
        ClaimsPrincipal principal = GetUserWithPermissions(PermissionEntityType.TestCase, PermissionLevel.Read);
        var field = new TestCaseField { FieldDefinitionId = 1, TestCaseId = 1, StringValue = "Test" };
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _fieldManager.UpsertTestCaseFieldAsync(principal, field));
    }

    /// <summary>
    /// Tests that attempting to upsert a test run field with only read permissions throws an <see cref="UnauthorizedAccessException"/>.
    /// </summary>
    [Fact]
    public async Task UpsertTestRunFieldAsync_WithOnlyReadPermission_ThrowsUnauthorizedException()
    {
        ClaimsPrincipal principal = GetUserWithPermissions(PermissionEntityType.TestRun, PermissionLevel.Read);
        var field = new TestRunField { FieldDefinitionId = 1, TestRunId = 1, StringValue = "Test" };
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _fieldManager.UpsertTestRunFieldAsync(principal, field));
    }

    /// <summary>
    /// Tests that attempting to upsert an issue field with only read permissions throws an <see cref="UnauthorizedAccessException"/>.
    /// </summary>
    [Fact]
    public async Task UpsertIssueFieldAsync_WithOnlyReadPermission_ThrowsUnauthorizedException()
    {
        ClaimsPrincipal principal = GetUserWithPermissions(PermissionEntityType.Issue, PermissionLevel.Read);
        var field = new IssueField { FieldDefinitionId = 1, LocalIssueId = 1, StringValue = "Test" };
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _fieldManager.UpsertIssueFieldAsync(principal, field));
    }

    /// <summary>
    /// Creates a <see cref="ClaimsPrincipal"/> with the specified permissions.
    /// </summary>
    /// <param name="entityType">The type of entity for which permissions are granted.</param>
    /// <param name="level">The level of permissions to grant.</param>
    /// <returns>A <see cref="ClaimsPrincipal"/> with the specified permissions.</returns>
    private static ClaimsPrincipal GetUserWithPermissions(PermissionEntityType entityType, PermissionLevel level)
    {
        return Impersonation.Impersonate(x =>
        {
            x.TenantId = TenantId;
            x.Email = x.UserName = UserName;
            x.Add(entityType, level);

            // Project read access is needed to read field definitions
            x.Add(PermissionEntityType.Project, PermissionLevel.Read);
        });
    }
}
