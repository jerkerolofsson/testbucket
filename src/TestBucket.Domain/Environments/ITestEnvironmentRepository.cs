using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Environments.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Environments;
public interface ITestEnvironmentRepository
{
    Task DeleteTestEnvironmentAsync(long id);
    Task AddTestEnvironmentAsync(TestEnvironment testEnvironment);
    Task UpdateTestEnvironmentAsync(TestEnvironment testEnvironment);
    Task<IReadOnlyList<TestEnvironment>> GetTestEnvironmentsAsync(string tenantId);

    Task<IReadOnlyList<TestEnvironment>> GetTestEnvironmentsAsync(IEnumerable<FilterSpecification<TestEnvironment>> filters);

    Task<TestEnvironment?> GetTestEnvironmentByIdAsync(string tenantId, long id);
}
