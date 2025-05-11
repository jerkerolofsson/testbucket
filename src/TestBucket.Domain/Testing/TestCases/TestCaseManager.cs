using System.Runtime.CompilerServices;

using Mediator;

using TestBucket.Domain.Fields;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Aggregates;
using TestBucket.Domain.Testing.Duplication;
using TestBucket.Domain.Testing.Events;
using TestBucket.Domain.Testing.Markdown;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.Specifications.TestCaseRuns;
using TestBucket.Domain.Testing.Specifications.TestCases;
using TestBucket.Domain.Testing.TestSuites;
using TestBucket.Domain.Traceability.Models;
using TestBucket.Domain.Traceability;

namespace TestBucket.Domain.Testing.TestCases
{
    public class TestCaseManager : ITestCaseManager
    {
        private readonly List<ITestCaseObserver> _testCaseObservers = new();
        private readonly IReadOnlyList<IMarkdownDetector> _markdownDetectors;
        private readonly ITestCaseRepository _testCaseRepo;
        private readonly IProjectRepository _projectRepo;
        private readonly ITestSuiteManager _testSuiteManager;
        private readonly IMediator _mediator;
        private readonly IFieldDefinitionManager _fieldDefinitionManager;

        public TestCaseManager(
            IEnumerable<IMarkdownDetector> markdownDetectors,
            ITestCaseRepository testCaseRepo, ITestSuiteManager testSuiteManager, IProjectRepository projectRepo, IMediator mediator, IFieldDefinitionManager fieldDefinitionManager)
        {
            _markdownDetectors = markdownDetectors.ToList();
            _testCaseRepo = testCaseRepo;
            _testSuiteManager = testSuiteManager;
            _projectRepo = projectRepo;
            _mediator = mediator;
            _fieldDefinitionManager = fieldDefinitionManager;
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
        /// Enumerates all items by fetching page-by-page
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async IAsyncEnumerable<long> SearchTestCaseIdsAsync(ClaimsPrincipal principal, SearchTestQuery query, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.TestCase, PermissionLevel.Read);

            //var fieldDefinitions = await _fieldDefinitionManager.GetDefinitionsAsync(principal, query.ProjectId, Contracts.Fields.FieldTarget.TestCase);
            FilterSpecification<TestCase>[] filters = [new FilterByTenant<TestCase>(principal.GetTenantIdOrThrow()), ..TestCaseFilterSpecificationBuilder.From(query)];

            foreach(var id in await _testCaseRepo.SearchTestCaseIdsAsync(filters))
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return id;
            }
        }


        /// <summary>
        /// Adds a test case
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="testCase"></param>
        /// <returns></returns>
        public async Task<TestCase> AddTestCaseAsync(ClaimsPrincipal principal, TestCase testCase)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.TestCase, PermissionLevel.Write);
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
            return testCase;
        }

        /// <summary>
        /// Deletes a test case
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="testCase"></param>
        /// <returns></returns>
        public async Task DeleteTestCaseAsync(ClaimsPrincipal principal, TestCase testCase)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.TestCase, PermissionLevel.Delete);
            principal.ThrowIfEntityTenantIsDifferent(testCase);
            await _testCaseRepo.DeleteTestCaseByIdAsync(testCase.Id);

            // Notify observers
            foreach (var observer in _testCaseObservers.ToList())
            {
                await observer.OnTestDeletedAsync(testCase);
            }
            await _mediator.Publish(new TestCaseDeletedEvent(testCase));
        }

        /// <summary>
        /// Returns a list of all items, starting with the root item until the test case including all folders in between
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="testCase"></param>
        /// <returns></returns>
        public async Task<IReadOnlyList<TestEntity>> ExpandUntilRootAsync(ClaimsPrincipal principal, TestCase testCase)
        {
            var tenantId = principal.GetTenantIdOrThrow();

            var result = new List<TestEntity>();
            result.Add(testCase);

            var folderId = testCase.TestSuiteFolderId;
            while (folderId is not null)
            {
                var folder = await _testCaseRepo.GetTestSuiteFolderByIdAsync(tenantId, folderId.Value);
                if(folder is not null)
                {
                    result.Add(folder);
                }
                folderId = folder?.ParentId;
            }

            var testSuite = await _testCaseRepo.GetTestSuiteByIdAsync(tenantId, testCase.TestSuiteId);

            result.Reverse();
            return result;
        }

        public async Task SaveTestCaseAsync(ClaimsPrincipal principal, TestCase testCase)
        {
            var tenantId = principal.GetTenantIdOrThrow();
            principal.ThrowIfEntityTenantIsDifferent(testCase);
            principal.ThrowIfNoPermission(PermissionEntityType.TestCase, PermissionLevel.Write);

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

        public async Task<TestCase> DuplicateTestCaseAsync(ClaimsPrincipal principal, TestCase testCase)
        {
            return await _mediator.Send(new DuplicateTestCaseRequest(principal, testCase));
        }

        public async Task<TestCase?> GetTestCaseByIdAsync(ClaimsPrincipal principal, long testCaseId)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.TestCase, PermissionLevel.Delete);
            var tenantId = principal.GetTenantIdOrThrow();
            return await _testCaseRepo.GetTestCaseByIdAsync(tenantId, testCaseId);
        }

        public async Task<TestCase?> GetTestCaseBySlugAsync(ClaimsPrincipal principal, string slug)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.TestCase, PermissionLevel.Delete);
            var tenantId = principal.GetTenantIdOrThrow();
            return await _testCaseRepo.GetTestCaseBySlugAsync(tenantId, slug);
        }

        public async Task<Dictionary<string, long>> GetTestCaseDistributionByFieldAsync(ClaimsPrincipal principal, SearchTestQuery query, long fieldDefinitionId)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.TestCase, PermissionLevel.Read);
            var tenantId = principal.GetTenantIdOrThrow();
            List<FilterSpecification<TestCase>> filters = TestCaseFilterSpecificationBuilder.From(query);
            filters.Add(new FilterByTenant<TestCase>(tenantId));
            return await _testCaseRepo.GetTestCaseDistributionByFieldAsync(filters, fieldDefinitionId);
        }


        public async Task<Dictionary<string, Dictionary<string,long>>> GetTestCaseCoverageMatrixByFieldAsync(ClaimsPrincipal principal, SearchTestQuery query, long fieldDefinitionId1, long fieldDefinitionId2)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.TestCase, PermissionLevel.Read);
            var tenantId = principal.GetTenantIdOrThrow();

            List<FilterSpecification<TestCase>> filters = TestCaseFilterSpecificationBuilder.From(query);
            filters.Add(new FilterByTenant<TestCase>(tenantId));
            return await _testCaseRepo.GetTestCaseCoverageMatrixByFieldAsync(filters, fieldDefinitionId1, fieldDefinitionId2);
        }

        /// <inheritdoc/>
        public async Task<Dictionary<long, TestExecutionResultSummary>> GetTestExecutionResultSummaryForRunsAsync(ClaimsPrincipal principal, IReadOnlyList<long> testRunsIds, SearchTestCaseRunQuery query)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.TestCaseRun, PermissionLevel.Read);
            var tenantId = principal.GetTenantIdOrThrow();
            List<FilterSpecification<TestCaseRun>> filters = TestCaseRunsFilterSpecificationBuilder.From(query);
            filters.Add(new FilterByTenant<TestCaseRun>(tenantId));

            return await _testCaseRepo.GetTestExecutionResultSummaryForRunsAsync(testRunsIds, filters);
        }

        public async Task<TraceabilityNode> DiscoverTraceabilityAsync(ClaimsPrincipal principal, TestCase testCase, int depth)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.TestCase, PermissionLevel.Read);
            principal.ThrowIfNoPermission(PermissionEntityType.Requirement, PermissionLevel.Read);

            return await _mediator.Send(new DiscoverTestCaseRelationshipsRequest(principal, testCase, depth));
        }

    }
}
