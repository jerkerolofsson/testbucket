using System.Security.Claims;
using TestBucket.Domain.ApiKeys.Validation;
namespace TestBucket.Domain.UnitTests.ApiKeys.Validation;

/// <summary>
/// Unit tests for the <see cref="PrincipalValidator"/> class.
/// </summary>
[UnitTest]
[FunctionalTest]
[Component("Identity")]
[EnrichedTest]
public class PrincipalValidatorTests
{
    /// <summary>
    /// Verifies that <see cref="PrincipalValidator.GetProjectId"/> returns the correct project id when present.
    /// </summary>
    [Fact]
    public void GetProjectId_ReturnsProjectId_WhenClaimExists()
    {
        var claims = new[] { new Claim("project", "123") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));
        var validator = new PrincipalValidator(principal);

        var result = validator.GetProjectId();

        Assert.Equal(123, result);
    }

    /// <summary>
    /// Verifies that <see cref="PrincipalValidator.GetProjectId"/> returns null when the claim is missing.
    /// </summary>
    [Fact]
    public void GetProjectId_ReturnsNull_WhenClaimMissing()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity());
        var validator = new PrincipalValidator(principal);

        var result = validator.GetProjectId();

        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that <see cref="PrincipalValidator.ThrowIfNoProjectId"/> throws when the project id claim is missing.
    /// </summary>
    [Fact]
    public void ThrowIfNoProjectId_Throws_WhenClaimMissing()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity());
        var validator = new PrincipalValidator(principal);

        Assert.Throws<UnauthorizedAccessException>(() => validator.ThrowIfNoProjectId());
    }

    /// <summary>
    /// Verifies that <see cref="PrincipalValidator.GetTestSuiteId"/> returns the correct test suite id when present.
    /// </summary>
    [Fact]
    public void GetTestSuiteId_ReturnsTestSuiteId_WhenClaimExists()
    {
        var claims = new[] { new Claim("testsuite", "456") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));
        var validator = new PrincipalValidator(principal);

        var result = validator.GetTestSuiteId();

        Assert.Equal(456, result);
    }

    /// <summary>
    /// Verifies that <see cref="PrincipalValidator.GetTestRunId"/> returns the correct test run id when present.
    /// </summary>
    [Fact]
    public void GetTestRunId_ReturnsTestRunId_WhenClaimExists()
    {
        var claims = new[] { new Claim("run", "789") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));
        var validator = new PrincipalValidator(principal);

        var result = validator.GetTestRunId();

        Assert.Equal(789, result);
    }

    /// <summary>
    /// Verifies that <see cref="PrincipalValidator.GetTenantId"/> returns the correct tenant id when present.
    /// </summary>
    [Fact]
    public void GetTenantId_ReturnsTenantId_WhenClaimExists()
    {
        var claims = new[] { new Claim("tenant", "tenant-abc") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));
        var validator = new PrincipalValidator(principal);

        var result = validator.GetTenantId();

        Assert.Equal("tenant-abc", result);
    }
}
