using System.Security.Claims;

using TestBucket.Domain.Environments.Models;

namespace TestBucket.Domain.Environments;
public interface ITestEnvironmentManager
{
    Task AddTestEnvironmentAsync(ClaimsPrincipal principal, TestEnvironment testEnvironment);
    Task DeleteTestEnvironmentAsync(ClaimsPrincipal principal, long id);
    Task<TestEnvironment?> GetDefaultTestEnvironmentAsync(ClaimsPrincipal principal, long projectId);
    Task<IReadOnlyList<TestEnvironment>> GetProjectTestEnvironmentsAsync(ClaimsPrincipal principal, long projectId);
    Task<TestEnvironment?> GetTestEnvironmentByIdAsync(ClaimsPrincipal principal, long id);
    Task<IReadOnlyList<TestEnvironment>> GetTestEnvironmentsAsync(ClaimsPrincipal principal);
    Task UpdateTestEnvironmentAsync(ClaimsPrincipal principal, TestEnvironment testEnvironment);
}