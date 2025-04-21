using Mediator;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.TestAccounts;
using TestBucket.Domain.TestAccounts.Models;
using TestBucket.Domain.TestResources.Specifications;

namespace TestBucket.Domain.TestAccounts.Allocation;
public record class ReleaseAccountsRequest(string LockOwner, string TenantId) : IRequest;

/// <summary>
/// Releases accounts that are locked by a specific owner
/// </summary>
public class ReleaseAccountsHandler : IRequestHandler<ReleaseAccountsRequest>
{
    private readonly ITestAccountRepository _resourceRepository;

    public ReleaseAccountsHandler(ITestAccountRepository resourceRepository)
    {
        _resourceRepository = resourceRepository;
    }

    public async ValueTask<Unit> Handle(ReleaseAccountsRequest request, CancellationToken cancellationToken)
    {
        FilterSpecification<TestAccount>[] filters = [
            new FilterByTenant<TestAccount>(request.TenantId),
            new FindLockedAccount(),
            new FindAccountByLockOwner(request.LockOwner)
       ];

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
