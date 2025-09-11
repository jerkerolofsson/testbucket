
using Mediator;

using TestBucket.Domain.Files.IntegrationEvents;
using TestBucket.Domain.Files.Models;

namespace TestBucket.Domain.Files
{
    internal class FileResourceManager : IFileResourceManager
    {
        private readonly IFileRepository _fileRepository;
        private readonly IMediator _mediator;
        private readonly List<IFileResourceObserver> _observers = [];

        public void AddObserver(IFileResourceObserver observer) => _observers.Add(observer);
        public void RemoveObserver(IFileResourceObserver observer) => _observers.Remove(observer);

        public FileResourceManager(IFileRepository fileRepository, IMediator mediator)
        {
            _fileRepository = fileRepository;
            _mediator = mediator;
        }

        /// <summary>
        /// Adds a file resource
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="resource"></param>
        /// <returns></returns>
        public async Task AddResourceAsync(ClaimsPrincipal principal, FileResource resource)
        {
            resource.TenantId = principal.GetTenantIdOrThrow();
            principal.ThrowIfNoPermission(resource);

            resource.Created = DateTimeOffset.UtcNow;
            await _fileRepository.AddResourceAsync(resource);

            await _mediator.Publish(new FileResourceAddedNotification(principal, resource));

            foreach(var observer in _observers)
            {
                await observer.OnAddedAsync(resource);
            }
        }

        public async Task<bool> DeleteResourceByIdAsync(ClaimsPrincipal principal, long id)
        {
            var file = await GetResourceByIdAsync(principal, id);
            if (file is null)
            {
                return false;
            }
            principal.ThrowIfNoPermission(file, PermissionLevel.Delete);

            await _fileRepository.DeleteResourceByIdAsync(principal.GetTenantIdOrThrow(), id);


            foreach (var observer in _observers)
            {
                await observer.OnDeletedAsync(file);
            }

            return true;
        }

        public async Task<IReadOnlyList<FileResource>> GetRequirementAttachmentsAsync(ClaimsPrincipal principal, long id)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.Requirement, PermissionLevel.Read);
            return await _fileRepository.GetRequirementAttachmentsAsync(principal.GetTenantIdOrThrow(), id);
        }

        public async Task<IReadOnlyList<FileResource>> GetRequirementSpecificationAttachmentsAsync(ClaimsPrincipal principal, long id)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.RequirementSpecification, PermissionLevel.Read);
            return await _fileRepository.GetRequirementSpecificationAttachmentsAsync(principal.GetTenantIdOrThrow(), id);
        }

        public async Task<FileResource?> GetResourceByIdAsync(ClaimsPrincipal principal, long id)
        {
            var resource = await _fileRepository.GetResourceByIdAsync(principal.GetTenantIdOrThrow(), id);
            if (resource is not null)
            {
                principal.ThrowIfNoPermission(resource, PermissionLevel.Read);
            }
            return resource;
        }

        public async Task<IReadOnlyList<FileResource>> GetTestCaseAttachmentsAsync(ClaimsPrincipal principal, long testCaseId)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.TestCase, PermissionLevel.Read);
            return await _fileRepository.GetTestCaseAttachmentsAsync(principal.GetTenantIdOrThrow(), testCaseId);
        }

        public async Task<IReadOnlyList<FileResource>> GetTestCaseRunAttachmentsAsync(ClaimsPrincipal principal, long testCaseRunId)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.TestCaseRun, PermissionLevel.Read);
            return await _fileRepository.GetTestCaseRunAttachmentsAsync(principal.GetTenantIdOrThrow(), testCaseRunId);
        }

        public async Task<IReadOnlyList<FileResource>> GetTestProjectAttachmentsAsync(ClaimsPrincipal principal, long testProjectId)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.Project, PermissionLevel.Read);
            return await _fileRepository.GetTestProjectAttachmentsAsync(principal.GetTenantIdOrThrow(), testProjectId);
        }

        public async Task<IReadOnlyList<FileResource>> GetTestRunAttachmentsAsync(ClaimsPrincipal principal, long id)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.TestRun, PermissionLevel.Read);
            return await _fileRepository.GetTestRunAttachmentsAsync(principal.GetTenantIdOrThrow(), id);
        }

        public async Task<IReadOnlyList<FileResource>> GetTestSuiteAttachmentsAsync(ClaimsPrincipal principal, long testSuiteId)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.TestSuite, PermissionLevel.Read);
            return await _fileRepository.GetTestSuiteAttachmentsAsync(principal.GetTenantIdOrThrow(), testSuiteId);
        }

        public async Task<IReadOnlyList<FileResource>> GetTestSuiteFolderAttachmentsAsync(ClaimsPrincipal principal, long testSuiteFolderId)
        {
            principal.ThrowIfNoPermission(PermissionEntityType.TestSuite, PermissionLevel.Read);
            return await _fileRepository.GetTestSuiteFolderAttachmentsAsync(principal.GetTenantIdOrThrow(), testSuiteFolderId);
        }

    }
}
