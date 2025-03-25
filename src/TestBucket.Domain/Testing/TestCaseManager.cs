using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.AI;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing
{
    public class TestCaseManager : ITestCaseManager
    {
        private readonly List<ITestCaseObserver> _testCaseObservers = new();
        private readonly IClassifier _classifier;
        private readonly ITestCaseRepository _testCaseRepo;
        private readonly IFieldManager _fieldManager;
        private readonly IFieldDefinitionManager _fieldDefinitionManager;

        public TestCaseManager(
            IClassifier classifier,
            ITestCaseRepository testCaseRepo, IFieldManager fieldManager, IFieldDefinitionManager fieldDefinitionManager)
        {
            _classifier = classifier;
            _testCaseRepo = testCaseRepo;
            _fieldManager = fieldManager;
            _fieldDefinitionManager = fieldDefinitionManager;
        }

        /// <summary>
        /// Adds an observer
        /// </summary>
        /// <param name="listener"></param>
        public void AddObserver(ITestCaseObserver observer) => _testCaseObservers.Add(observer);

        /// <summary>
        /// Removes an observer
        /// </summary>
        /// <param name="observer"></param>
        public void RemoveObserver(ITestCaseObserver observer) => _testCaseObservers.Remove(observer);

        /// <summary>
        /// Adds a test case
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="testCase"></param>
        /// <returns></returns>
        public async Task AddTestCaseAsync(ClaimsPrincipal principal, TestCase testCase)
        {
            testCase.TenantId = principal.GetTentantIdOrThrow();

            testCase.Modified = testCase.Created = DateTimeOffset.UtcNow;
            testCase.CreatedBy = testCase.ModifiedBy = principal.Identity?.Name ?? throw new InvalidOperationException("User not authenticated");
            testCase.ClassificationRequired = testCase.Description is not null && testCase.Description.Length > 0;

            await _testCaseRepo.AddTestCaseAsync(testCase);

            // Notify observers
            foreach (var observer in _testCaseObservers.ToList())
            {
                await observer.OnTestCreatedAsync(testCase);
            }
        }

        /// <summary>
        /// Deletes a test case
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="testCase"></param>
        /// <returns></returns>
        public async Task DeleteTestCaseAsync(ClaimsPrincipal principal, TestCase testCase)
        {
            principal.ThrowIfEntityTenantIsDifferent(testCase);
            await _testCaseRepo.DeleteTestCaseByIdAsync(testCase.Id);

            // Notify observers
            foreach (var observer in _testCaseObservers.ToList())
            {
                await observer.OnTestDeletedAsync(testCase);
            }
        }

        public async Task SaveTestCaseAsync(ClaimsPrincipal principal, TestCase testCase)
        {
            principal.ThrowIfEntityTenantIsDifferent(testCase);

            testCase.Modified = DateTimeOffset.UtcNow;
            testCase.ModifiedBy = principal.Identity?.Name ?? throw new InvalidOperationException("User not authenticated");
            testCase.ClassificationRequired = testCase.Description is not null && testCase.Description.Length > 0;

            await _testCaseRepo.UpdateTestCaseAsync(testCase);

            // Notify observers
            foreach (var observer in _testCaseObservers.ToList())
            {
                await observer.OnTestSavedAsync(testCase);
            }
        }
    }
}
