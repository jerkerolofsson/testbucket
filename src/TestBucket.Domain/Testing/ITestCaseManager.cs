using System.Security.Claims;

using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing;
public interface ITestCaseManager
{
    /// <summary>
    /// Adds an observer that will receive notifications when things are changing
    /// </summary>
    /// <param name="observer"></param>
    void AddObserver(ITestCaseObserver observer);

    /// <summary>
    /// Adds a test case
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="testCase"></param>
    /// <returns></returns>
    Task AddTestCaseAsync(ClaimsPrincipal principal, TestCase testCase);

    /// <summary>
    /// Deletes a test case
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="testCase"></param>
    /// <returns></returns>
    Task DeleteTestCaseAsync(ClaimsPrincipal principal, TestCase testCase);

    /// <summary>
    /// Removes an observer
    /// </summary>
    /// <param name="observer"></param>
    void RemoveObserver(ITestCaseObserver observer);

    /// <summary>
    /// Saves a test case
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="testCase"></param>
    /// <returns></returns>
    Task SaveTestCaseAsync(ClaimsPrincipal principal, TestCase testCase);
}