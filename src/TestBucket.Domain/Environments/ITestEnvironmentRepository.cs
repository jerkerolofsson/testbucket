using TestBucket.Domain.Environments.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Environments;

/// <summary>
/// Persistance storage for test-environments
/// </summary>
public interface ITestEnvironmentRepository
{
    /// <summary>
    /// Deletes an environment
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task DeleteTestEnvironmentAsync(long id);

    /// <summary>
    /// Adds an environment
    /// </summary>
    /// <param name="testEnvironment"></param>
    /// <returns></returns>
    Task AddTestEnvironmentAsync(TestEnvironment testEnvironment);

    /// <summary>
    /// Updates an environment
    /// </summary>
    /// <param name="testEnvironment"></param>
    /// <returns></returns>
    Task UpdateTestEnvironmentAsync(TestEnvironment testEnvironment);

    /// <summary>
    /// Gets all environments for a tenant
    /// </summary>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    Task<IReadOnlyList<TestEnvironment>> GetTestEnvironmentsAsync(string tenantId);

    /// <summary>
    /// Searches for environments
    /// </summary>
    /// <param name="filters"></param>
    /// <returns></returns>
    Task<IReadOnlyList<TestEnvironment>> GetTestEnvironmentsAsync(IEnumerable<FilterSpecification<TestEnvironment>> filters);

    /// <summary>
    /// Gets an environment by ID
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<TestEnvironment?> GetTestEnvironmentByIdAsync(string tenantId, long id);
}
