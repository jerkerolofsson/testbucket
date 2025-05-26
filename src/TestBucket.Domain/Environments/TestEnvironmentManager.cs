using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Environments.Models;
using TestBucket.Domain.Environments.Specifications;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Environments;
internal class TestEnvironmentManager : ITestEnvironmentManager
{
    private readonly ITestEnvironmentRepository _repository;

    public TestEnvironmentManager(ITestEnvironmentRepository repo)
    {
        _repository = repo;
    }
    public async Task AddTestEnvironmentAsync(ClaimsPrincipal principal, TestEnvironment testEnvironment)
    {
        testEnvironment.TenantId = principal.GetTenantIdOrThrow();
        testEnvironment.Modified = testEnvironment.Created = DateTimeOffset.UtcNow;
        testEnvironment.CreatedBy = testEnvironment.ModifiedBy = principal.Identity?.Name ?? throw new InvalidOperationException("User not authenticated");
        await _repository.AddTestEnvironmentAsync(testEnvironment);
    }

    public async Task DeleteTestEnvironmentAsync(ClaimsPrincipal principal, long id)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        var environment = await _repository.GetTestEnvironmentByIdAsync(tenantId, id);
        if (environment is not null)
        {
            await _repository.DeleteTestEnvironmentAsync(id);
        }
    }

    public async Task<TestEnvironment?> GetDefaultTestEnvironmentAsync(ClaimsPrincipal principal, long projectId)
    {
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
        var tenantId = principal.GetTenantIdOrThrow();
        return await _repository.GetTestEnvironmentByIdAsync(tenantId, id);
    }
    public async Task<IReadOnlyList<TestEnvironment>> GetTestEnvironmentsAsync(ClaimsPrincipal principal)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        return await _repository.GetTestEnvironmentsAsync(tenantId);
    }
    public async Task<IReadOnlyList<TestEnvironment>> GetProjectTestEnvironmentsAsync(ClaimsPrincipal principal, long projectId)
    {
        var tenantId = principal.GetTenantIdOrThrow();

        FilterSpecification<TestEnvironment>[] filters = [
            new FilterByTenant<TestEnvironment>(tenantId),
            new FilterByProject<TestEnvironment>(projectId)
            ];

        var environments = await _repository.GetTestEnvironmentsAsync(filters);
        return environments.ToList();
    }

    public async Task UpdateTestEnvironmentAsync(ClaimsPrincipal principal, TestEnvironment testEnvironment)
    {
        principal.ThrowIfEntityTenantIsDifferent(testEnvironment);
        testEnvironment.Modified = DateTimeOffset.UtcNow;
        testEnvironment.ModifiedBy = principal.Identity?.Name ?? throw new InvalidOperationException("User not authenticated");
        await _repository.UpdateTestEnvironmentAsync(testEnvironment);
    }
}
