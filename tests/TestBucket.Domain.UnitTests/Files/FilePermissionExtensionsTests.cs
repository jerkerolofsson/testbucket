using System.Security.Claims;

using TestBucket.Domain.Files;
using TestBucket.Domain.Files.Models;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Identity.Permissions;

namespace TestBucket.Domain.UnitTests.Files;

/// <summary>
/// Unit tests for <see cref="FilePermissionExtensions"/> extension methods.
/// Verifies permission checks for file resources based on tenant and entity permissions.
/// </summary>
[EnrichedTest]
[UnitTest]
[Component("Files")]
public class FilePermissionExtensionsTests
{
    /// <summary>
    /// Creates a <see cref="ClaimsPrincipal"/> with the specified tenant ID and permissions.
    /// </summary>
    /// <param name="tenantId">The tenant ID to assign to the principal.</param>
    /// <param name="permissions">An array of permission entity types and levels to assign.</param>
    /// <returns>A configured <see cref="ClaimsPrincipal"/>.</returns>
    private ClaimsPrincipal CreatePrincipalWithPermissions(string tenantId, params (PermissionEntityType, PermissionLevel)[] permissions)
    {
        return Impersonation.Impersonate(builder =>
        {
            builder.TenantId = tenantId;
            builder.UserName = builder.Email = "user@acme.com";
            foreach (var permission in permissions)
            {
                builder.Add(permission.Item1, permission.Item2);
            }
        });
    }

    /// <summary>
    /// Creates a <see cref="FileResource"/> for the specified tenant.
    /// </summary>
    /// <param name="tenantId">The tenant ID to assign to the file resource.</param>
    /// <returns>A new <see cref="FileResource"/> instance.</returns>
    private FileResource CreateFileResource(string tenantId) =>
        new FileResource
        {
            TenantId = tenantId,
            ContentType = "application/octet-stream",
            Data = new byte[1],
            Name = "file.txt"
        };

    /// <summary>
    /// Verifies that an <see cref="UnauthorizedAccessException"/> is thrown if the principal's tenant does not match the file's tenant.
    /// </summary>
    [Fact]
    public void ThrowsIfTenantIsDifferent()
    {
        var principal = CreatePrincipalWithPermissions("tenantA");
        var file = CreateFileResource("tenantB");
        Assert.Throws<UnauthorizedAccessException>(() => principal.ThrowIfNoPermission(file));
    }

    /// <summary>
    /// Verifies that an <see cref="UnauthorizedAccessException"/> is thrown if the principal lacks permission for a test case file.
    /// </summary>
    [Fact]
    public void ThrowsIfNoTestCasePermission()
    {
        var principal = CreatePrincipalWithPermissions("tenantA");
        var file = CreateFileResource("tenantA");
        file.TestCaseId = 1;
        Assert.Throws<UnauthorizedAccessException>(() => principal.ThrowIfNoPermission(file));
    }

    /// <summary>
    /// Verifies that permission checks succeed when the principal has test case permission.
    /// </summary>
    [Fact]
    public void SucceedsWithTestCasePermission()
    {
        var principal = CreatePrincipalWithPermissions("tenantA", (PermissionEntityType.TestCase, PermissionLevel.Read));
        var file = CreateFileResource("tenantA");
        file.TestCaseId = 1;
        principal.ThrowIfNoPermission(file);
    }

    /// <summary>
    /// Verifies that an <see cref="UnauthorizedAccessException"/> is thrown if the principal lacks permission for a test run file.
    /// </summary>
    [Fact]
    public void ThrowsIfNoTestRunPermission()
    {
        var principal = CreatePrincipalWithPermissions("tenantA");
        var file = CreateFileResource("tenantA");
        file.TestRunId = 1;
        Assert.Throws<UnauthorizedAccessException>(() => principal.ThrowIfNoPermission(file));
    }

    /// <summary>
    /// Verifies that permission checks succeed when the principal has test run permission.
    /// </summary>
    [Fact]
    public void SucceedsWithTestRunPermission()
    {
        var principal = CreatePrincipalWithPermissions("tenantA", (PermissionEntityType.TestRun, PermissionLevel.Read));
        var file = CreateFileResource("tenantA");
        file.TestRunId = 1;
        principal.ThrowIfNoPermission(file);
    }

    /// <summary>
    /// Verifies that an <see cref="UnauthorizedAccessException"/> is thrown if the principal lacks permission for an architecture file.
    /// </summary>
    [Fact]
    public void ThrowsIfNoArchitecturePermission()
    {
        var principal = CreatePrincipalWithPermissions("tenantA");
        var file = CreateFileResource("tenantA");
        file.ComponentId = 1;
        Assert.Throws<UnauthorizedAccessException>(() => principal.ThrowIfNoPermission(file));
    }

    /// <summary>
    /// Verifies that permission checks succeed when the principal has architecture permission.
    /// </summary>
    [Fact]
    public void SucceedsWithArchitecturePermission()
    {
        var principal = CreatePrincipalWithPermissions("tenantA", (PermissionEntityType.Architecture, PermissionLevel.Read));
        var file = CreateFileResource("tenantA");
        file.ComponentId = 1;
        principal.ThrowIfNoPermission(file);
    }

    /// <summary>
    /// Verifies that an <see cref="UnauthorizedAccessException"/> is thrown if the principal lacks permission for a requirement file.
    /// </summary>
    [Fact]
    public void ThrowsIfNoRequirementPermission()
    {
        var principal = CreatePrincipalWithPermissions("tenantA");
        var file = CreateFileResource("tenantA");
        file.RequirementId = 1;
        Assert.Throws<UnauthorizedAccessException>(() => principal.ThrowIfNoPermission(file));
    }

    /// <summary>
    /// Verifies that permission checks succeed when the principal has requirement permission.
    /// </summary>
    [Fact]
    public void SucceedsWithRequirementPermission()
    {
        var principal = CreatePrincipalWithPermissions("tenantA", (PermissionEntityType.Requirement, PermissionLevel.Read));
        var file = CreateFileResource("tenantA");
        file.RequirementId = 1;
        principal.ThrowIfNoPermission(file);
    }

    /// <summary>
    /// Verifies that an <see cref="UnauthorizedAccessException"/> is thrown if the principal lacks permission for a project file.
    /// </summary>
    [Fact]
    public void ThrowsIfNoProjectPermission()
    {
        var principal = CreatePrincipalWithPermissions("tenantA");
        var file = CreateFileResource("tenantA");
        file.TestProjectId = 1;
        Assert.Throws<UnauthorizedAccessException>(() => principal.ThrowIfNoPermission(file));
    }

    /// <summary>
    /// Verifies that permission checks succeed when the principal has project permission.
    /// </summary>
    [Fact]
    public void SucceedsWithProjectPermission()
    {
        var principal = CreatePrincipalWithPermissions("tenantA", (PermissionEntityType.Project, PermissionLevel.Read));
        var file = CreateFileResource("tenantA");
        file.TestProjectId = 1;
        principal.ThrowIfNoPermission(file);
    }
}