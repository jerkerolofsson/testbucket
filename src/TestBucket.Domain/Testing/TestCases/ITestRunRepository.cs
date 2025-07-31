using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.TestCases;
public interface ITestRunRepository
{
    /// <summary>
    /// Returns a test run from an ID
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<TestRun?> GetTestRunByIdAsync(string tenantId, long id);

    /// <summary>
    /// Adds a run
    /// </summary>
    /// <param name="testRun"></param>
    /// <returns></returns>
    Task AddTestRunAsync(TestRun testRun);

    /// <summary>
    /// Searches for test runs
    /// </summary>
    /// <returns></returns>
    Task<PagedResult<TestRun>> SearchTestRunsAsync(IReadOnlyList<FilterSpecification<TestRun>> filters, int offset, int count);

    /// <summary>
    /// Deletes a test run
    /// </summary>
    /// <returns></returns>
    Task DeleteTestRunAsync(TestRun testRun);

    /// <summary>
    /// Updates a test run / saves changes
    /// </summary>
    /// <param name="testRun"></param>
    /// <returns></returns>
    Task UpdateTestRunAsync(TestRun testRun);
}

