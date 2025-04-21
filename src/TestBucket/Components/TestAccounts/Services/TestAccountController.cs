
using Microsoft.Extensions.Localization;

using TestBucket.Components.TestAccounts.Dialogs;
using TestBucket.Components.Tests.Dialogs;
using TestBucket.Domain.TestAccounts;
using TestBucket.Domain.TestAccounts.Models;
using TestBucket.Localization;

namespace TestBucket.Components.TestAccounts.Services;

internal class TestAccountController : TenantBaseService
{
    private readonly ITestAccountManager _manager;
    private readonly IStringLocalizer<SharedStrings> _loc;
    private readonly IDialogService _dialogService;

    public TestAccountController(
        ITestAccountManager manager,
        IStringLocalizer<SharedStrings> loc,
        AuthenticationStateProvider authenticationStateProvider,
        IDialogService dialogService) : base(authenticationStateProvider)
    {
        _manager = manager;
        _loc = loc;
        _dialogService = dialogService;
    }

    public async Task<TestAccount?> GetAccountByIdAsync(long id)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _manager.GetAccountByIdAsync(principal, id);
    }

    public async Task LockAsync(TestAccount account)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        account.LockOwner = principal.Identity?.Name ?? "user";
        account.LockExpires = null;
        account.Locked = true;
        await _manager.UpdateAsync(principal, account);
    }
    public async Task UpdateAsync(TestAccount account)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _manager.UpdateAsync(principal, account);
    }
    public async Task UnlockAsync(TestAccount account)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        account.LockOwner = null;
        account.LockExpires = null;
        account.Locked = false;
        await _manager.UpdateAsync(principal, account);
    }
    public async Task DeleteAsync(TestAccount account)
    {
        var result = await _dialogService.ShowMessageBox(
            _loc["confirm-delete-title"],
            _loc["confirm-delete-message"],
            _loc["ok"],
            _loc["cancel"]);
        if(result == true)
        {
            var principal = await GetUserClaimsPrincipalAsync();
            await _manager.DeleteAsync(principal, account);
        }
    }

    public async Task AddAsync()
    {
        var dialog = await _dialogService.ShowAsync<CreateAccountDialog>(null, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;

        if (result?.Data is TestAccount testAccount)
        {
            var principal = await GetUserClaimsPrincipalAsync();
            await _manager.AddAsync(principal, testAccount);
        }
    }

    public async Task<PagedResult<TestAccount>> GetAccountsAsync(int offset, int count)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _manager.BrowseAsync(principal, offset, count);
    }
}
