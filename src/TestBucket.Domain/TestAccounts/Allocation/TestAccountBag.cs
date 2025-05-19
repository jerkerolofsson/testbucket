using TestBucket.Domain.TestAccounts.Models;

namespace TestBucket.Domain.TestAccounts.Allocation;

/// <summary>
/// This contains resources resources which are unlocked when the test completes
/// </summary>
public class TestAccountBag
{
    private readonly List<TestAccount> _resources = [];
    private readonly ClaimsPrincipal _principal;
    private readonly ITestAccountManager _manager;

    /// <summary>
    /// Gets locked accounts
    /// </summary>
    public IReadOnlyList<TestAccount> Accounts => _resources;

    public TestAccountBag(ClaimsPrincipal principal, ITestAccountManager manager)
    {
        _principal = principal;
        _manager = manager;
    }

    public async ValueTask AddAsync(TestAccount resource, DateTimeOffset lockExpires, string lockOwner)
    {
        _resources.Add(resource);
        resource.Locked = true;
        resource.LockOwner = lockOwner;
        resource.LockExpires = lockExpires;
        await _manager.UpdateAsync(_principal, resource);
    }

    public void ResolveVariables(TestAccount account, Dictionary<string, string> variables)
    {
        //foreach(var resource in _resources)
        {
            var type = account.Type;
            var resourcesByType = _resources.Where(x => x.Type == type).ToList();
            var index = resourcesByType.IndexOf(account);

            var key = $"accounts__{type}__{index}";

            foreach(var attribute in account.Variables)
            {
                var fullName = key + "__" + attribute.Key;
                variables[fullName] = attribute.Value;  
            }
        }
    }
}
