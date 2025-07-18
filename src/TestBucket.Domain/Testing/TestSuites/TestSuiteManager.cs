﻿using Mediator;

using TestBucket.Domain.Testing.Events;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.TestSuites.Search;

namespace TestBucket.Domain.Testing.TestSuites
{
    class TestSuiteManager : ITestSuiteManager
    {
        private readonly TimeProvider _timeProvider;
        private readonly ITestCaseRepository _testCaseRepository;
        private readonly List<ITestSuiteObserver> _testSuiteObservers = new();
        private readonly List<ITestSuiteFolderObserver> _testSuiteFolderObservers = new();
        private readonly IMediator _mediator;

        public TestSuiteManager(
            TimeProvider timeProvider,
            ITestCaseRepository testCaseRepository, IMediator mediator)
        {
            _timeProvider = timeProvider;
            _testCaseRepository = testCaseRepository;
            _mediator = mediator;
        }

        #region Observers
        /// <summary>
        /// Adds an observer
        /// </summary>
        /// <param name="listener"></param>
        public void AddFolderObserver(ITestSuiteFolderObserver observer) => _testSuiteFolderObservers.Add(observer);

        /// <summary>
        /// Removes an observer
        /// </summary>
        /// <param name="observer"></param>
        public void RemoveFolderObserver(ITestSuiteFolderObserver observer) => _testSuiteFolderObservers.Remove(observer);


        /// <summary>
        /// Adds an observer
        /// </summary>
        /// <param name="listener"></param>
        public void AddObserver(ITestSuiteObserver observer) => _testSuiteObservers.Add(observer);

        /// <summary>
        /// Removes an observer
        /// </summary>
        /// <param name="observer"></param>
        public void RemoveObserver(ITestSuiteObserver observer) => _testSuiteObservers.Remove(observer);
        #endregion Observers

        #region Test Suites
        public async Task<TestSuite?> GetTestSuiteByIdAsync(ClaimsPrincipal principal, long id)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.TestSuite, PermissionLevel.Read);
            var tenantId = principal.GetTenantIdOrThrow();
            return await _testCaseRepository.GetTestSuiteByIdAsync(tenantId, id);
        }

        public async Task<TestSuiteFolder[]> GetTestSuiteFoldersAsync(ClaimsPrincipal principal, long? projectId, long testSuiteId, long? parentFolderId)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.TestSuite, PermissionLevel.Read);
            var tenantId = principal.GetTenantIdOrThrow();
            return await _testCaseRepository.GetTestSuiteFoldersAsync(tenantId, projectId, testSuiteId, parentFolderId);
        }


        /// <summary>
        /// Adds a test suite
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="teamId"></param>
        /// <param name="projectId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<TestSuite> AddTestSuiteAsync(ClaimsPrincipal principal, long? teamId, long? projectId, string name, string? ciCdSystem, string? ciCdRef)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.TestSuite, PermissionLevel.Write);
            var suite = new TestSuite { Name = name, TestProjectId = projectId, TeamId = teamId, DefaultCiCdRef = ciCdRef, CiCdSystem = ciCdSystem };
            return await AddTestSuiteAsync(principal, suite);
        }



        /// <summary>
        /// Adds a test suite
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="suite"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<TestSuite> AddTestSuiteAsync(ClaimsPrincipal principal, TestSuite suite)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.TestSuite, PermissionLevel.Write);

            suite.TenantId = principal.GetTenantIdOrThrow();
            suite.Modified = suite.Created = _timeProvider.GetUtcNow();
            suite.CreatedBy = suite.ModifiedBy = principal.Identity?.Name ?? throw new InvalidOperationException("User not authenticated");
            suite.DefaultCiCdRef ??= "main";
            suite.CiCdWorkflow ??= "test.yml";

            if (suite.AddPipelinesStartedFromOutside is null)
            {
                // Default, pull mode for CI/CD artifacts..
                suite.AddPipelinesStartedFromOutside = true;
            }

            await _testCaseRepository.AddTestSuiteAsync(suite);

            foreach (var observer in _testSuiteObservers)
            {
                await observer.OnTestSuiteCreatedAsync(suite);
            }
            await _mediator.Publish(new TestSuiteCreatedEvent(suite));

            return suite;
        }

        /// <summary>
        /// Updates a test suite / saves changes
        /// </summary>
        /// <param name="principal">principal</param>
        /// <param name="suite"></param>
        /// <returns></returns>
        public async Task UpdateTestSuiteAsync(ClaimsPrincipal principal, TestSuite suite)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.TestSuite, PermissionLevel.Write);

            var tenantId = principal.ThrowIfEntityTenantIsDifferent(suite);
            var existingTestSuite = await _testCaseRepository.GetTestSuiteByIdAsync(tenantId, suite.Id);
            if(existingTestSuite is null)
            {
                throw new ArgumentException("Test suite was not found in database");
            }

            suite.Modified = _timeProvider.GetUtcNow();
            suite.ModifiedBy = principal.Identity?.Name ?? throw new InvalidOperationException("User not authenticated");

            await _testCaseRepository.UpdateTestSuiteAsync(suite);

            if(existingTestSuite?.FolderId != suite.FolderId)
            {
                foreach (var observer in _testSuiteObservers)
                {
                    await observer.OnTestSuiteMovedAsync(suite);
                }
            }

            // Invoke saved listeners
            foreach (var observer in _testSuiteObservers)
            {
                await observer.OnTestSuiteSavedAsync(suite);
            }
        }

        /// <summary>
        /// Deletes a test sutie
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="testSuiteId"></param>
        /// <returns></returns>
        public async Task DeleteTestSuiteByIdAsync(ClaimsPrincipal principal, long testSuiteId)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.TestSuite, PermissionLevel.Delete);

            var tenantId = principal.GetTenantIdOrThrow();
            var suite = await _testCaseRepository.GetTestSuiteByIdAsync(tenantId, testSuiteId);
            if(suite is null)
            {
                return;
            }

            await _testCaseRepository.DeleteTestSuiteByIdAsync(tenantId, testSuiteId);

            foreach(var observer in _testSuiteObservers)
            {
                await observer.OnTestSuiteDeletedAsync(suite);
            }
            await _mediator.Publish(new TestSuiteDeletedEvent(suite));
        }

        /// <summary>
        /// Searches for test suites
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<PagedResult<TestSuite>> SearchTestSuitesAsync(ClaimsPrincipal principal, SearchTestSuiteQuery query) 
        {
            principal.ThrowIfNoPermission(PermissionEntityType.TestSuite, PermissionLevel.Read);
            var tenantId = principal.GetTenantIdOrThrow();
            return await _testCaseRepository.SearchTestSuitesAsync(tenantId, query);
        }

        /// <summary>
        /// Returns a test suite by slug
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="slug"></param>
        /// <returns></returns>
        public async Task<TestSuite?> GetTestSuiteBySlugAsync(ClaimsPrincipal principal, long? projectId, string slug)
        {
            var tenantId = principal.GetTenantIdOrThrow();
            principal.ThrowIfNoPermission(PermissionEntityType.TestSuite, PermissionLevel.Read);
            return await _testCaseRepository.GetTestSuiteBySlugAsync(tenantId, projectId, slug);
        }
        /// <summary>
        /// Returns a test suite by name or null if not found
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="teamId"></param>
        /// <param name="projectId"></param>
        /// <param name="suiteName"></param>
        /// <returns></returns>
        public async Task<TestSuite?> GetTestSuiteByNameAsync(ClaimsPrincipal principal, long? teamId, long? projectId, string suiteName) 
        {
            var tenantId = principal.GetTenantIdOrThrow();
            principal.ThrowIfNoPermission(PermissionEntityType.TestSuite, PermissionLevel.Read);
            return await _testCaseRepository.GetTestSuiteByNameAsync(tenantId, teamId, projectId, suiteName);
        }
        #endregion Test Suites

        #region Test Suite Folders

        /// <summary>
        /// Adds a new test suite folder
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="projectId"></param>
        /// <param name="testSuiteId"></param>
        /// <param name="parentFolderId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<TestSuiteFolder> AddTestSuiteFolderAsync(ClaimsPrincipal principal, long? projectId, long testSuiteId, long? parentFolderId, string name)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.TestSuite, PermissionLevel.Write);
            var tenantId = principal.GetTenantIdOrThrow();

            var folder = await _testCaseRepository.AddTestSuiteFolderAsync(tenantId, projectId, testSuiteId, parentFolderId, name);

            // Notify observers
            foreach (var observer in _testSuiteFolderObservers)
            {
                await observer.OnTestSuiteFolderCreatedAsync(folder);
            }

            return folder;
        }

        /// <summary>
        /// Saves changes to a test suite folder
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task SaveTestSuiteFolderAsync(ClaimsPrincipal principal, TestSuiteFolder folder)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.TestSuite, PermissionLevel.Delete);
            principal.ThrowIfEntityTenantIsDifferent(folder);

            folder.Modified = _timeProvider.GetUtcNow();
            folder.ModifiedBy = principal.Identity?.Name ?? throw new InvalidOperationException("User not authenticated");

            await _testCaseRepository.UpdateTestSuiteFolderAsync(folder);

            // Notify observers
            foreach (var observer in _testSuiteFolderObservers.ToList())
            {
                await observer.OnTestSuiteFolderSavedAsync(folder);
            }
        }

        /// <summary>
        /// Deletes a test suite folder by id
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="folderId"></param>
        /// <returns></returns>
        public async Task DeleteTestSuiteFolderByIdAsync(ClaimsPrincipal principal, long folderId)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.TestSuite, PermissionLevel.Delete);
            var tenantId = principal.GetTenantIdOrThrow();
            var folder = await _testCaseRepository.GetTestSuiteFolderByIdAsync(tenantId, folderId);
            if (folder is not null)
            {
                await DeleteTestSuiteFolderAsync(principal, folder);
            }
        }

        /// <summary>
        /// Deletes a test suite folder
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        public async Task DeleteTestSuiteFolderAsync(ClaimsPrincipal principal, TestSuiteFolder folder)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.TestSuite, PermissionLevel.Delete);
            principal.ThrowIfEntityTenantIsDifferent(folder);
            var tenantId = principal.GetTenantIdOrThrow();
            await _testCaseRepository.DeleteFolderByIdAsync(tenantId, folder.Id);

            // Notify observers
            foreach (var observer in _testSuiteFolderObservers.ToList())
            {
                await observer.OnTestSuiteFolderDeletedAsync(folder);
            }
        }

        public async Task<TestSuiteFolder?> GetTestSuiteFolderByIdAsync(ClaimsPrincipal principal, long folderId)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.TestSuite, PermissionLevel.Read);
            var tenantId = principal.GetTenantIdOrThrow();
            return await _testCaseRepository.GetTestSuiteFolderByIdAsync(tenantId, folderId);
        }

        #endregion Test Suite Folders
    }
}
