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
    /// Duplicates a test case
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="testCase"></param>
    /// <returns></returns>
    Task<TestCase> DuplicateTestCaseAsync(ClaimsPrincipal principal, TestCase testCase);

    /// <summary>
    /// Returns a list of all items, starting with the root item until the test case including all folders in between
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="testCase"></param>
    /// <returns></returns>
    Task<IReadOnlyList<TestEntity>> ExpandUntilRootAsync(ClaimsPrincipal principal, TestCase testCase);

    /// <summary>
    /// Returns a test case by ID
    /// </summary>
    /// <param name="user"></param>
    /// <param name="testCaseId"></param>
    /// <returns></returns>
    Task<TestCase?> GetTestCaseByIdAsync(ClaimsPrincipal user, long testCaseId);

    /// <summary>
    /// Returns a test case by slug
    /// </summary>
    /// <param name="user"></param>
    /// <param name="slug"></param>
    /// <returns></returns>
    Task<TestCase?> GetTestCaseBySlugAsync(ClaimsPrincipal user, string slug);

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