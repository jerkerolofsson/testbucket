using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.Files.IntegrationEvents;
using TestBucket.Domain.Files.Models;

namespace TestBucket.Domain.Files
{
    internal class FileResourceManager : IFileResourceManager
    {
        private readonly IFileRepository _fileRepository;
        private readonly IMediator _mediator;

        public FileResourceManager(IFileRepository fileRepository, IMediator mediator)
        {
            _fileRepository = fileRepository;
            _mediator = mediator;
        }

        public async Task AddResourceAsync(ClaimsPrincipal principal, FileResource resource)
        {
            resource.TenantId = principal.GetTenantIdOrThrow();
            resource.Created = DateTimeOffset.UtcNow;
            await _fileRepository.AddResourceAsync(resource);

            await _mediator.Publish(new FileResourceAddedNotification(principal, resource));
        }

    }
}
