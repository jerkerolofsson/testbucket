

using Microsoft.Extensions.Localization;

using TestBucket.Components.AI.Dialogs;
using TestBucket.Domain.AI.Mcp;
using TestBucket.Domain.AI.Mcp.Models;
using TestBucket.Localization;

namespace TestBucket.Components.AI.Controllers;

internal class McpController : TenantBaseService
{
    private readonly IMcpServerManager _mcpServerManager;
    private readonly IStringLocalizer<SharedStrings> _loc;
    private readonly IDialogService _dialogService;

    public McpController(AuthenticationStateProvider authenticationStateProvider, IMcpServerManager mcpServerManager, IDialogService dialogService, IStringLocalizer<SharedStrings> loc) : base(authenticationStateProvider)
    {
        _mcpServerManager = mcpServerManager;
        _dialogService = dialogService;
        _loc = loc;
    }

    public async Task<McpServerRegistration?> AddMcpServerAsync(TestProject? project)
    {
        var hasPermission = await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.McpServer, PermissionLevel.Write);
        if (!hasPermission)
            return null;


        var parameters = new DialogParameters<AddMcpServerDialog>
        {
            { x=>x.Project, project }
        };
        var dialog = await _dialogService.ShowAsync<AddMcpServerDialog>(null, parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        if (result?.Data is McpServerRegistration registration)
        {
            var principal = await GetUserClaimsPrincipalAsync();
            await _mcpServerManager.AddMcpServerRegistrationAsync(principal, registration);
            return registration;
        }
        return null;
    }

    public async Task<IReadOnlyList<McpServerRegistration>> GetMcpServerRegistrationsAsync()
    {
        var hasPermission = await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.McpServer, PermissionLevel.Read);
        if (!hasPermission)
            return [];

        var user = await GetUserClaimsPrincipalAsync();
        return await _mcpServerManager.GetAllMcpServerRegistationsAsync(user);
    }
    public async Task<IReadOnlyList<McpServerRegistration>> GetMcpServerRegistrationsAsync(long projectId)
    {
        var hasPermission = await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.McpServer, PermissionLevel.Read);
        if (!hasPermission)
            return [];

        var user = await GetUserClaimsPrincipalAsync();
        return await _mcpServerManager.GetUserMcpServerRegistationsAsync(user, projectId);
    }

    internal async Task ClearUserInput(McpServerRegistration mcpRegistration)
    {
        if(mcpRegistration.TestProjectId is null)
        {
            return;
        }

        var user = await GetUserClaimsPrincipalAsync();
        await _mcpServerManager.ClearUserInputsAsync(user, mcpRegistration.TestProjectId.Value, mcpRegistration.Id);
    }

    internal async Task UpdateMcpServerAsync(McpServerRegistration mcpRegistration)
    {
        var hasPermission = await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.McpServer, PermissionLevel.Write);
        if (!hasPermission)
            return;

        var user = await GetUserClaimsPrincipalAsync();
        await _mcpServerManager.UpdateMcpServerRegistrationAsync(user, mcpRegistration);
    }
    internal async Task DeleteMcpServerAsync(McpServerRegistration mcpRegistration)
    {
        var hasPermission = await ShowErrorIfNoPermissionAsync(_loc, _dialogService, PermissionEntityType.McpServer, PermissionLevel.Delete);
        if (!hasPermission)
            return;

        var user = await GetUserClaimsPrincipalAsync();
        await _mcpServerManager.DeleteMcpServerRegistrationAsync(user, mcpRegistration);
    }
}
