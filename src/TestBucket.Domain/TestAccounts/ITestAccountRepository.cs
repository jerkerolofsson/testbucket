using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.TestAccounts.Models;

namespace TestBucket.Domain.TestAccounts;
public interface ITestAccountRepository
{
    Task DeleteAsync(long id);
    Task AddAsync(TestAccount account);
    Task UpdateAsync(TestAccount account);
    Task<PagedResult<TestAccount>> SearchAsync(FilterSpecification<TestAccount>[] filters, int offset, int count);
    Task<TestAccount?> GetAccountByIdAsync(string tenantId, long id);
}
