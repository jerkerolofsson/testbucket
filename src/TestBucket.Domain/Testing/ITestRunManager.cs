using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing;
public interface ITestRunManager
{
    void AddObserver(ITestRunObserver observer);
    void RemoveObserver(ITestRunObserver observer);

    #region Test Runs
    Task AddTestCaseRunAsync(ClaimsPrincipal principal, TestCaseRun testCaseRun);
    Task AddTestRunAsync(ClaimsPrincipal principal, TestRun testRun);
    Task DeleteTestRunAsync(ClaimsPrincipal principal, TestRun testRun);
    #endregion Test Runs

    #region Test Case Runs
    Task SaveTestCaseRunAsync(ClaimsPrincipal principal, TestCaseRun testCaseRun);
    Task<PagedResult<TestCaseRun>> SearchTestCaseRunsAsync(ClaimsPrincipal principal, SearchTestCaseRunQuery query);
    Task<PagedResult<TestRun>> SearchTestRunsAsync(ClaimsPrincipal principal, SearchTestRunQuery query);

    #endregion Test Case Runs
}
