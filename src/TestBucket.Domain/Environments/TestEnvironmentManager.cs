using TestBucket.Domain.Environments.Models;
using TestBucket.Domain.Environments.Specifications;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Environments;
internal class TestEnvironmentManager : ITestEnvironmentManager
{
    private readonly ITestEnvironmentRepository _repository;
    private readonly TimeProvider _timeProvider;

    public TestEnvironmentManager(ITestEnvironmentRepository repo, TimeProvider timeProvider)
    {
        _repository = repo;
        _timeProvider = timeProvider;
    }
    public async Task AddTestEnvironmentAsync(ClaimsPrincipal principal, TestEnvironment testEnvironment)
    {
        ArgumentNullException.ThrowIfNull(principal);
        ArgumentNullException.ThrowIfNull(testEnvironment);

        principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Write);

        testEnvironment.TenantId = principal.GetTenantIdOrThrow();
        testEnvironment.Modified = testEnvironment.Created = _timeProvider.GetUtcNow();
        testEnvironment.CreatedBy = testEnvironment.ModifiedBy = principal.Identity?.Name ?? throw new InvalidOperationException("User not authenticated");
        await _repository.AddTestEnvironmentAsync(testEnvironment);
    }

    public async Task DeleteTestEnvironmentAsync(ClaimsPrincipal principal, long id)
    {
        ArgumentNullException.ThrowIfNull(principal);
        principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Write);

        var tenantId = principal.GetTenantIdOrThrow();
        var environment = await _repository.GetTestEnvironmentByIdAsync(tenantId, id);
        if (environment is not null)
        {
            await _repository.DeleteTestEnvironmentAsync(id);
        }
    }

    public async Task<TestEnvironment?> GetDefaultTestEnvironmentAsync(ClaimsPrincipal principal, long projectId)
    {
        ArgumentNullException.ThrowIfNull(principal);
        principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Read);
        var tenantId = principal.GetTenantIdOrThrow();

        FilterSpecification<TestEnvironment>[] filters = [
            new FilterByTenant<TestEnvironment>(tenantId),
            new FilterByProject<TestEnvironment>(projectId),
            new FilterEnvironmentByDefault()
            ];

        var environments = await _repository.GetTestEnvironmentsAsync(filters);
        return environments.FirstOrDefault();
    }

    public async Task<TestEnvironment?> GetTestEnvironmentByIdAsync(ClaimsPrincipal principal, long id)
    {
        ArgumentNullException.ThrowIfNull(principal);
        principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Read);
        var tenantId = principal.GetTenantIdOrThrow();
        return await _repository.GetTestEnvironmentByIdAsync(tenantId, id);
    }
    public async Task<IReadOnlyList<TestEnvironment>> GetTestEnvironmentsAsync(ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);
        principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Read);

        var tenantId = principal.GetTenantIdOrThrow();
        return await _repository.GetTestEnvironmentsAsync(tenantId);
    }
    public async Task<IReadOnlyList<TestEnvironment>> GetProjectTestEnvironmentsAsync(ClaimsPrincipal principal, long projectId)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Read);

        FilterSpecification<TestEnvironment>[] filters = [
            new FilterByTenant<TestEnvironment>(tenantId),
            new FilterByProject<TestEnvironment>(projectId)
            ];

        var environments = await _repository.GetTestEnvironmentsAsync(filters);
        return environments.ToList();
    }

    public async Task UpdateTestEnvironmentAsync(ClaimsPrincipal principal, TestEnvironment testEnvironment)
    {
        ArgumentNullException.ThrowIfNull(principal);
        ArgumentNullException.ThrowIfNull(testEnvironment);

        principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Write);
        principal.ThrowIfEntityTenantIsDifferent(testEnvironment);

        testEnvironment.Modified = _timeProvider.GetUtcNow();
        testEnvironment.ModifiedBy = principal.Identity?.Name ?? throw new InvalidOperationException("User not authenticated");
        await _repository.UpdateTestEnvironmentAsync(testEnvironment);
    }
}
