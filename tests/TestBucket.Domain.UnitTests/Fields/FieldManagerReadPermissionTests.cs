using System.Security.Claims;

using Microsoft.Extensions.DependencyInjection;

using Quartz;

using TestBucket.Domain.Fields;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.UnitTests.Fakes;

namespace TestBucket.Domain.UnitTests.Fields;

/// <summary>
/// Unit tests for verifying read permissions in the FieldManager class.
/// </summary>
[Feature("Fields")]
[UnitTest]
[Component("Fields")]
[EnrichedTest]
[SecurityTest]
public class FieldManagerReadPermissionTests
{
    private readonly IFieldManager _fieldManager;
    private const string TenantId = "tenant-1";
    private const string UserName = "user@internet.com";

    /// <summary>
    /// Initializes a new instance of the <see cref="FieldManagerReadPermissionTests"/> class.
    /// Sets up the required services and dependencies for testing.
    /// </summary>
    public FieldManagerReadPermissionTests()
    {
        var services = new ServiceCollection();
        services.AddMemoryCache();
        services.AddLogging();
        services.AddScoped<IFieldRepository, FakeFieldRepository>();
        services.AddScoped<IFieldManager, FieldManager>();
        services.AddScoped<IFieldDefinitionManager, FieldDefinitionManager>();
        services.AddMediator(options => options.ServiceLifetime = ServiceLifetime.Scoped);
        services.AddSingleton<ISchedulerFactory>(NSubstitute.Substitute.For<ISchedulerFactory>());
        _fieldManager = services.BuildServiceProvider().GetRequiredService<IFieldManager>();
    }

    /// <summary>
    /// Verifies that <see cref="IFieldManager.GetRequirementFieldsAsync"/> throws an <see cref="UnauthorizedAccessException"/>
    /// when the user does not have read permissions.
    /// </summary>
    [Fact]
    public async Task GetRequirementFieldsAsync_WithoutReadPermission_ThrowsUnauthorizedException()
    {
        ClaimsPrincipal principal = GetUserWithoutAnyPermissions();
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _fieldManager.GetRequirementFieldsAsync(principal, 1, []));
    }

    /// <summary>
    /// Verifies that <see cref="IFieldManager.GetTestCaseFieldsAsync"/> throws an <see cref="UnauthorizedAccessException"/>
    /// when the user does not have read permissions.
    /// </summary>
    [Fact]
    public async Task GetTestCaseFieldsAsync_WithoutReadPermission_ThrowsUnauthorizedException()
    {
        ClaimsPrincipal principal = GetUserWithoutAnyPermissions();
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _fieldManager.GetTestCaseFieldsAsync(principal, 1, []));
    }

    /// <summary>
    /// Verifies that <see cref="IFieldManager.GetTestCaseRunFieldsAsync"/> throws an <see cref="UnauthorizedAccessException"/>
    /// when the user does not have read permissions.
    /// </summary>
    [Fact]

    public async Task GetTestCaseRunFieldsAsync_WithoutReadPermission_ThrowsUnauthorizedException()
    {
        ClaimsPrincipal principal = GetUserWithoutAnyPermissions();
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _fieldManager.GetTestCaseRunFieldsAsync(principal, 1, 1, []));
    }

    /// <summary>
    /// Verifies that <see cref="IFieldManager.GetTestRunFieldsAsync"/> throws an <see cref="UnauthorizedAccessException"/>
    /// when the user does not have read permissions.
    /// </summary>
    [Fact]
    public async Task GetTestRunFieldsAsync_WithoutReadPermission_ThrowsUnauthorizedException()
    {
        ClaimsPrincipal principal = GetUserWithoutAnyPermissions();
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _fieldManager.GetTestRunFieldsAsync(principal, 1, []));
    }

    /// <summary>
    /// Verifies that <see cref="IFieldManager.GetTestRunFieldAsync"/> throws an <see cref="UnauthorizedAccessException"/>
    /// when the user does not have read permissions.
    /// </summary>
    [Fact]
    public async Task GetTestRunFieldAsync_WithoutReadPermission_ThrowsUnauthorizedException()
    {
        ClaimsPrincipal principal = GetUserWithoutAnyPermissions();
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _fieldManager.GetTestRunFieldAsync(principal, 1, 1, x=>true, ""));
    }

    /// <summary>
    /// Verifies that <see cref="IFieldManager.GetIssueFieldsAsync"/> throws an <see cref="UnauthorizedAccessException"/>
    /// when the user does not have read permissions.
    /// </summary>
    [Fact]
    public async Task GetIssueFieldsAsync_WithoutReadPermission_ThrowsUnauthorizedException()
    {
        ClaimsPrincipal principal = GetUserWithoutAnyPermissions();
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _fieldManager.GetIssueFieldsAsync(principal, 1, []));
    }

    /// <summary>
    /// Verifies that <see cref="IFieldManager.GetIssueFieldAsync"/> throws an <see cref="UnauthorizedAccessException"/>
    /// when the user does not have read permissions.
    /// </summary>
    [Fact]
    public async Task GetIssueFieldAsync_WithoutReadPermission_ThrowsUnauthorizedException()
    {
        ClaimsPrincipal principal = GetUserWithoutAnyPermissions();
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _fieldManager.GetIssueFieldAsync(principal, 1, 1, x=>true, "aa"));
    }

    /// <summary>
    /// Verifies that <see cref="IFieldManager.GetIssueFieldAsync"/> does not throw an <see cref="UnauthorizedAccessException"/>
    /// when the user has read permissions.
    /// </summary>
    [Fact]
    public async Task GetIssueFieldAsync_WithReadPermission_DoesNotThrowUnauthorizedException()
    {
        ClaimsPrincipal principal = GetUserWithPermissions(PermissionEntityType.Issue, PermissionLevel.Read);
        await _fieldManager.GetIssueFieldAsync(principal, 1, 1, x => true, "aa");
    }

    /// <summary>
    /// Verifies that <see cref="IFieldManager.GetIssueFieldsAsync"/> does not throw an <see cref="UnauthorizedAccessException"/>
    /// when the user has read permissions.
    /// </summary>
    [Fact]
    public async Task GetIssueFieldsAsync_WithReadPermission_DoesNotThrowUnauthorizedException()
    {
        ClaimsPrincipal principal = GetUserWithPermissions(PermissionEntityType.Issue, PermissionLevel.Read);
        await _fieldManager.GetIssueFieldsAsync(principal, 1, []);
    }

    /// <summary>
    /// Verifies that <see cref="IFieldManager.GetRequirementFieldsAsync"/> does not throw an <see cref="UnauthorizedAccessException"/>
    /// when the user has read permissions.
    /// </summary>
    [Fact]
    public async Task GetRequirementFieldsAsync_WithReadPermission_DoesNotThrowUnauthorizedException()
    {
        ClaimsPrincipal principal = GetUserWithPermissions(PermissionEntityType.Requirement, PermissionLevel.Read);
        await _fieldManager.GetRequirementFieldsAsync(principal, 1, []);
    }

    /// <summary>
    /// Verifies that <see cref="IFieldManager.GetTestRunFieldAsync"/> does not throw an <see cref="UnauthorizedAccessException"/>
    /// when the user has read permissions.
    /// </summary>
    [Fact]
    public async Task GetTestRunFieldAsync_WithReadPermission_DoesNotThrowUnauthorizedException()
    {
        ClaimsPrincipal principal = GetUserWithPermissions(PermissionEntityType.TestRun, PermissionLevel.Read);
        await _fieldManager.GetTestRunFieldAsync(principal, 1, 1, x => true, "");
    }

    /// <summary>
    /// Verifies that <see cref="IFieldManager.GetTestRunFieldsAsync"/> does not throw an <see cref="UnauthorizedAccessException"/>
    /// when the user has read permissions.
    /// </summary>
    [Fact]
    public async Task GetTestRunFieldsAsync_WithReadPermission_DoesNotThrowUnauthorizedException()
    {
        ClaimsPrincipal principal = GetUserWithPermissions(PermissionEntityType.TestRun, PermissionLevel.Read);
        await _fieldManager.GetTestRunFieldsAsync(principal, 1, []);
    }

    /// <summary>
    /// Verifies that <see cref="IFieldManager.GetTestCaseFieldsAsync"/> does not throw an <see cref="UnauthorizedAccessException"/>
    /// when the user has read permissions.
    /// </summary>
    [Fact]
    public async Task GetTestCaseFieldsAsync_WithReadPermission_DoesNotThrowUnauthorizedException()
    {
        ClaimsPrincipal principal = GetUserWithPermissions(PermissionEntityType.TestCase, PermissionLevel.Read);
        await _fieldManager.GetTestCaseFieldsAsync(principal, 1, []);
    }

    /// <summary>
    /// Verifies that <see cref="IFieldManager.GetTestCaseRunFieldsAsync"/> does not throw an <see cref="UnauthorizedAccessException"/>
    /// when the user has read permissions.
    /// </summary>
    [Fact]
    public async Task GetTestCaseRunFieldsAsync_WithReadPermission_DoesNotThrowUnauthorizedException()
    {
        ClaimsPrincipal principal = GetUserWithPermissions(PermissionEntityType.TestCaseRun, PermissionLevel.Read);
        await _fieldManager.GetTestCaseRunFieldsAsync(principal, 1,1, []);
    }

    /// <summary>
    /// Creates a user with the specified permissions for testing.
    /// </summary>
    /// <param name="entityType">The type of entity for which permissions are granted.</param>
    /// <param name="level">The level of permissions granted.</param>
    /// <returns>A <see cref="ClaimsPrincipal"/> representing the user with the specified permissions.</returns>
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

    /// <summary>
    /// Creates a user without any permissions for testing.
    /// </summary>
    /// <returns>A <see cref="ClaimsPrincipal"/> representing the user without any permissions.</returns>
    private static ClaimsPrincipal GetUserWithoutAnyPermissions()
    {
        return Impersonation.Impersonate(x =>
        {
            x.TenantId = TenantId;
            x.Email = x.UserName = UserName;
        });
    }
}
