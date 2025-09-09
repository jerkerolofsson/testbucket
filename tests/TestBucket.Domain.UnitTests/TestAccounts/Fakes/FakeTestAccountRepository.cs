namespace TestBucket.Domain.UnitTests.TestAccounts.Fakes;
using System.Collections.Concurrent;

using TestBucket.Contracts;
using TestBucket.Domain.TestAccounts;
using TestBucket.Domain.TestAccounts.Models;

internal class FakeTestAccountRepository : ITestAccountRepository
{
    private readonly ConcurrentDictionary<long, TestAccount> _accounts = new();

    private long _idCounter = 1;

    public Task DeleteAsync(long id)
    {
        _accounts.TryRemove(id, out _);
        return Task.CompletedTask;
    }

    public Task AddAsync(TestAccount account)
    {
        account.Id = _idCounter++;
        _accounts[account.Id] = account;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(TestAccount account)
    {
        if (_accounts.ContainsKey(account.Id))
        {
            _accounts[account.Id] = account;
        }
        return Task.CompletedTask;
    }

    public Task<PagedResult<TestAccount>> SearchAsync(FilterSpecification<TestAccount>[] filters, int offset, int count)
    {
        var filteredAccounts = _accounts.Values.AsQueryable();

        foreach (var filter in filters)
        {
            filteredAccounts = filteredAccounts.Where(filter.Expression);
        }

        var pagedAccounts = filteredAccounts
            .Skip(offset)
            .Take(count)
            .ToArray();

        return Task.FromResult(new PagedResult<TestAccount>
        {
            Items = pagedAccounts,
            TotalCount = filteredAccounts.Count()
        });
    }

    public Task<TestAccount?> GetAccountByIdAsync(string tenantId, long id)
    {
        _accounts.TryGetValue(id, out var account);
        return Task.FromResult(account);
    }
}
