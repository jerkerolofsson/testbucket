using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.AI;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Tenants.Models;
using TestBucket.Domain.Testing.Events;
using TestBucket.Domain.Testing.Markdown;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing
{
    public class TestCaseManager : ITestCaseManager
    {
        private readonly List<ITestCaseObserver> _testCaseObservers = new();
        private readonly IReadOnlyList<IMarkdownDetector> _markdownDetectors;
        private readonly ITestCaseRepository _testCaseRepo;
        private readonly IProjectRepository _projectRepo;
        private readonly ITestSuiteManager _testSuiteManager;
        private readonly IMediator _mediator;

        public TestCaseManager(
            IEnumerable<IMarkdownDetector> markdownDetectors,
            ITestCaseRepository testCaseRepo, ITestSuiteManager testSuiteManager, IProjectRepository projectRepo, IMediator mediator)
        {
            _markdownDetectors = markdownDetectors.ToList();
            _testCaseRepo = testCaseRepo;
            _testSuiteManager = testSuiteManager;
            _projectRepo = projectRepo;
            _mediator = mediator;
        }

        /// <summary>
        /// Adds an observer
        /// </summary>
        /// <param name="observer"></param>
        public void AddObserver(ITestCaseObserver observer) => _testCaseObservers.Add(observer);

        /// <summary>
        /// Removes an observer
        /// </summary>
        /// <param name="observer"></param>
        public void RemoveObserver(ITestCaseObserver observer) => _testCaseObservers.Remove(observer);

        private async Task CreateTestCaseFoldersAsync(ClaimsPrincipal principal, TestCase testCase)
        {
            // Create folder path if a path was specified for the test case
            // If a folder path was not specified, just use the parent folder
            if (!string.IsNullOrEmpty(testCase.Path))
            {
                var tenantId = principal.GetTenantIdOrThrow();

                long? parentId = testCase.TestSuiteFolderId;
                foreach (var folderName in testCase.Path.Split('/'))
                {
                    var folder = await _testCaseRepo.GetTestSuiteFolderByNameAsync(tenantId, testCase.TestSuiteId, parentId, folderName);
                    if (folder is null)
                    {
                        folder = await _testSuiteManager.AddTestSuiteFolderAsync(principal, testCase.TestProjectId, testCase.TestSuiteId, parentId, folderName);
                    }
                    parentId = folder?.Id;
                }
                testCase.TestSuiteFolderId = parentId;
                testCase.Path = "";
            }
        }


        /// <summary>
        /// Adds a test case
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="testCase"></param>
        /// <returns></returns>
        public async Task AddTestCaseAsync(ClaimsPrincipal principal, TestCase testCase)
        {
            testCase.TenantId = principal.GetTenantIdOrThrow();
            testCase.Modified = testCase.Created = DateTimeOffset.UtcNow;
            testCase.CreatedBy = testCase.ModifiedBy = principal.Identity?.Name ?? throw new InvalidOperationException("User not authenticated");
            testCase.ClassificationRequired = testCase.Description is not null && testCase.Description.Length > 0;

            await AssignTeamIfNotAssignedAsync(testCase, testCase.TenantId);
            await CreateTestCaseFoldersAsync(principal, testCase);

            if (testCase.TeamId is null && testCase.TestProjectId is not null)
            {
                var project = await _projectRepo.GetProjectByIdAsync(testCase.TenantId, testCase.TestProjectId.Value);
                testCase.TeamId = project?.TeamId;
            }
            await DetectThingsWithMarkdownDetectorsAsync(principal, testCase);
            await _testCaseRepo.AddTestCaseAsync(testCase);

            // Notify observers
            foreach (var observer in _testCaseObservers.ToList())
            {
                await observer.OnTestCreatedAsync(testCase);
            }
            await _mediator.Publish(new TestCaseCreatedEvent(testCase));
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
            await _mediator.Publish(new TestCaseDeletedEvent(testCase));
        }

        public async Task SaveTestCaseAsync(ClaimsPrincipal principal, TestCase testCase)
        {
            var tenantId = principal.GetTenantIdOrThrow();
            principal.ThrowIfEntityTenantIsDifferent(testCase);

            await DetectThingsWithMarkdownDetectorsAsync(principal, testCase);

            await AssignTeamIfNotAssignedAsync(testCase, tenantId);

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

        private async Task DetectThingsWithMarkdownDetectorsAsync(ClaimsPrincipal principal, TestCase testCase)
        {
            foreach (var detector in _markdownDetectors)
            {
                await detector.ProcessAsync(principal, testCase);
            }
        }

        private async Task AssignTeamIfNotAssignedAsync(TestCase testCase, string tenantId)
        {
            if (testCase.TeamId is null && testCase.TestProjectId is not null)
            {
                var project = await _projectRepo.GetProjectByIdAsync(tenantId, testCase.TestProjectId.Value);
                testCase.TeamId = project?.TeamId;
            }
        }
    }
}
