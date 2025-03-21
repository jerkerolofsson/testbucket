using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing;
internal class TestRunManager : ITestRunManager
{
    private readonly List<ITestRunObserver> _testRunObservers = new();

    private readonly ITestCaseRepository _testCaseRepo;

    public TestRunManager(ITestCaseRepository testCaseRepo)
    {
        _testCaseRepo = testCaseRepo;
    }

    /// <summary>
    /// Adds an observer
    /// </summary>
    /// <param name="listener"></param>
    public void AddObserver(ITestRunObserver observer) => _testRunObservers.Add(observer);

    /// <summary>
    /// Removes an observer
    /// </summary>
    /// <param name="observer"></param>
    public void RemoveObserver(ITestRunObserver observer) => _testRunObservers.Remove(observer);

    public async Task AddTestRunAsync(ClaimsPrincipal principal, TestRun testRun)
    {
        testRun.TenantId = principal.GetTentantIdOrThrow();

        testRun.Modified = testRun.Created = DateTimeOffset.UtcNow;
        testRun.CreatedBy = testRun.ModifiedBy = principal.Identity?.Name ?? throw new InvalidOperationException("User not authenticated");

        await _testCaseRepo.AddTestRunAsync(testRun);

        // Notify observers
        foreach (var observer in _testRunObservers.ToList())
        {
            await observer.OnRunCreatedAsync(testRun);
        }
    }

    public async Task DeleteTestRunAsync(ClaimsPrincipal principal, TestRun testRun)
    {
        principal.GetTentantIdOrThrow(testRun);
        await _testCaseRepo.DeleteTestRunByIdAsync(testRun.Id);
    }

    public async Task AddTestCaseRunAsync(ClaimsPrincipal principal, TestCaseRun testCaseRun)
    {
        testCaseRun.TenantId = principal.GetTentantIdOrThrow();

        testCaseRun.Modified = testCaseRun.Created = DateTimeOffset.UtcNow;
        testCaseRun.CreatedBy = testCaseRun.ModifiedBy = principal.Identity?.Name ?? throw new InvalidOperationException("User not authenticated");

        await _testCaseRepo.AddTestCaseRunAsync(testCaseRun);

        //// Notify observers
        //foreach (var observer in _testRunObservers.ToList())
        //{
        //    await observer.OnRunCreatedAsync(testRun);
        //}
    }

    public async Task SaveTestCaseRunAsync(ClaimsPrincipal principal, TestCaseRun testCaseRun)
    {
        principal.GetTentantIdOrThrow(testCaseRun);

        testCaseRun.Modified =  DateTimeOffset.UtcNow;
        testCaseRun.ModifiedBy = principal.Identity?.Name ?? throw new InvalidOperationException("User not authenticated");

        await _testCaseRepo.UpdateTestCaseRunAsync(testCaseRun);
    }
}
