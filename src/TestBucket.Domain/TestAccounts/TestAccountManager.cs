using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.TestAccounts.Models;

namespace TestBucket.Domain.TestAccounts;
internal class TestAccountManager : ITestAccountManager
{
    private readonly ITestAccountRepository _repository;

    public TestAccountManager(ITestAccountRepository repository)
    {
        _repository = repository;
    }

    public async Task UpdateAsync(ClaimsPrincipal principal, TestAccount account)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.TestAccount, PermissionLevel.Write);
        principal.ThrowIfEntityTenantIsDifferent(account.TenantId);

        account.TenantId = principal.GetTenantIdOrThrow();
        account.CreatedBy = principal.Identity?.Name ?? throw new Exception("Missing user identity");
        account.ModifiedBy = account.CreatedBy;
        account.Created = DateTimeOffset.UtcNow;
        account.Modified = DateTimeOffset.UtcNow;
        await _repository.UpdateAsync(account);
    }

    public async Task AddAsync(ClaimsPrincipal principal, TestAccount account)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.TestAccount, PermissionLevel.Write);

        account.TenantId = principal.GetTenantIdOrThrow();
        account.CreatedBy = principal.Identity?.Name ?? throw new Exception("Missing user identity");
        account.ModifiedBy = account.CreatedBy;
        account.Created = DateTimeOffset.UtcNow;
        account.Modified = DateTimeOffset.UtcNow;
        await _repository.AddAsync(account);
    }

    public async Task<PagedResult<TestAccount>> BrowseAsync(ClaimsPrincipal principal, int offset, int count)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.TestAccount, PermissionLevel.Read);

        FilterSpecification<TestAccount>[] filters = [new FilterByTenant<TestAccount>(principal.GetTenantIdOrThrow())];
        return await _repository.SearchAsync(filters, offset, count);
    }

    public async Task DeleteAsync(ClaimsPrincipal principal, TestAccount account)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.TestAccount, PermissionLevel.Delete);

        principal.ThrowIfEntityTenantIsDifferent(account.TenantId);
        await _repository.DeleteAsync(account.Id);
    }

    public async Task<TestAccount?> GetAccountByIdAsync(ClaimsPrincipal principal, long id)
    {
        var tenantId = principal.GetTenantIdOrThrow();
        principal.ThrowIfNoPermission(PermissionEntityType.TestAccount, PermissionLevel.Read);
        return await _repository.GetAccountByIdAsync(tenantId, id);
    }
}
