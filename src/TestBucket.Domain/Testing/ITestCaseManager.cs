using System.Security.Claims;

using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing;
public interface ITestCaseManager
{
    void AddObserver(ITestCaseObserver observer);
    Task AddTestCaseAsync(ClaimsPrincipal principal, TestCase testCase);

    /// <summary>
    /// Deletes a test case
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="testCase"></param>
    /// <returns></returns>
    Task DeleteTestCaseAsync(ClaimsPrincipal principal, TestCase testCase);
    void RemoveObserver(ITestCaseObserver observer);
    Task SaveTestCaseAsync(ClaimsPrincipal principal, TestCase testCase);
}