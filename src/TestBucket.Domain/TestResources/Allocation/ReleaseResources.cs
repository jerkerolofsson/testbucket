using Mediator;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.TestResources.Models;

using TestBucket.Domain.TestResources.Specifications;

namespace TestBucket.Domain.TestResources.Allocation;
public record class ReleaseResourcesRequest(string LockOwner, string TenantId) : IRequest;

/// <summary>
/// Releases resources that are locked by a specific owner
/// </summary>
public class ReleaseResourcesHandler : IRequestHandler<ReleaseResourcesRequest>
{
    private readonly ITestResourceRepository _resourceRepository;

    public ReleaseResourcesHandler(ITestResourceRepository resourceRepository)
    {
        _resourceRepository = resourceRepository;
    }

    public async ValueTask<Unit> Handle(ReleaseResourcesRequest request, CancellationToken cancellationToken)
    {
        FilterSpecification<TestResource>[] filters = [
            new FilterByTenant<TestResource>(request.TenantId),
            new FindLockedResource(),
            new FindResourceByLockOwner(request.LockOwner)
       ];

        // Note: Don't increment offset as we are filtering by unlocked..
        int offset = 0;
        int pageSize = 10;
        while (true)
        {
            var result = await _resourceRepository.SearchAsync(filters, offset, pageSize);
            foreach(var resource in result.Items)
            {
                resource.Locked = false;
                resource.LockOwner = null;
                resource.LockExpires = null;
                await _resourceRepository.UpdateAsync(resource);
            }

            if (pageSize != result.Items.Length)
            {
                break;
            }
        }
        return Unit.Value;
    }
}
