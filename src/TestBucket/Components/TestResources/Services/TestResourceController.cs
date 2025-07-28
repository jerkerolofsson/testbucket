
using TestBucket.Components.TestResources.Dialogs;
using TestBucket.Components.Tests.TestCases.Dialogs;
using TestBucket.Domain.Shared;
using TestBucket.Domain.TestResources;
using TestBucket.Domain.TestResources.Models;

namespace TestBucket.Components.TestResources.Services;

internal class TestResourceController : TenantBaseService
{
    private readonly ITestResourceManager _manager;
    private readonly IDialogService _dialogService;

    public TestResourceController(
        ITestResourceManager manager,
        AuthenticationStateProvider authenticationStateProvider,
        IDialogService dialogService) : base(authenticationStateProvider)
    {
        _manager = manager;
        _dialogService = dialogService;
    }

    public async Task<TestResource?> PickResourceAsync()
    {
        var principal = await GetUserClaimsPrincipalAsync();
        string tenantId = principal.GetTenantIdOrThrow();

        var parameters = new DialogParameters<PickResourceDialog>()
        {
        };

        var dialog = await _dialogService.ShowAsync<PickResourceDialog>(null, parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        if (result?.Data is TestResource testResource)
        {
            return testResource;
        }
        return null;
    }

    public async Task UpdateAsync(TestResource resource)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _manager.UpdateAsync(principal, resource);
    }

    public async Task DeleteAsync(TestResource resource)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _manager.DeleteAsync(principal, resource);
    }

    public async Task LockAsync(TestResource resource)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        resource.Locked = true;
        resource.LockExpires = null;
        resource.LockOwner = principal.Identity?.Name;
        await _manager.UpdateAsync(principal, resource);
    }
    public async Task UnlockAsync(TestResource resource)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        resource.Locked = false;
        resource.LockExpires = null;
        resource.LockOwner = null;
        await _manager.UpdateAsync(principal, resource);
    }
    public async Task<PagedResult<TestResource>> GetResourcesAsync(int offset, int count)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _manager.BrowseAsync(principal, offset, count);
    }
    public async Task<TestResource?> GetResourceByIdAsync(long id)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _manager.GetByIdAsync(principal, id);
    }
}
