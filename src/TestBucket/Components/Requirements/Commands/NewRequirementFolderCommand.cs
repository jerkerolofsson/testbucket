using Microsoft.Extensions.Localization;

using TestBucket.Components.Requirements.Services;
using TestBucket.Components.Tests.Services;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Keyboard;
using TestBucket.Localization;

namespace TestBucket.Components.Tests.Requiremnts.Commands;

internal class NewRequirementFolderCommand : ICommand
{
    private readonly AppNavigationManager _appNavigationManager;
    private readonly RequirementBrowser _browser;
    private readonly IStringLocalizer<SharedStrings> _loc;

    public NewRequirementFolderCommand(AppNavigationManager appNavigationManager, RequirementBrowser browser, IStringLocalizer<SharedStrings> loc)
    {
        _appNavigationManager = appNavigationManager;
        _browser = browser;
        _loc = loc;
    }

    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.TestSuite;
    public PermissionLevel? RequiredLevel => PermissionLevel.ReadWrite;
    public bool Enabled => _appNavigationManager.State.SelectedRequirementSpecification is not null;
    public string Id => "new-folder";
    public string Name => _loc["new-folder"];
    public string Description => _loc["new-folder-description"];

    public KeyboardBinding? DefaultKeyboardBinding => new KeyboardBinding() { CommandId = Id, Key = "F7", ModifierKeys = ModifierKey.None };
    public string? Icon => Icons.Material.Filled.CreateNewFolder;
    public string[] ContextMenuTypes => ["RequirementSpecification", "RequirementSpecificationFolder"];

    public async ValueTask ExecuteAsync()
    {
        var specification = _appNavigationManager.State.SelectedRequirementSpecification;
        if (specification is null)
        {
            return;
        }
        var projectId = specification.TestProjectId;
        if (projectId is null)
        {
            return;
        }
        await _browser.AddFolderAsync(projectId.Value, specification.Id, _appNavigationManager.State.SelectedRequirementSpecificationFolder?.Id);
        //await _browser.AddTestSuiteFolderAsync(projectId.Value, suite.Id, _appNavigationManager.State.SelectedTestSuiteFolder?.Id);
    }
}
