using Microsoft.Extensions.Localization;

using NGitLab.Models;

using TestBucket.Components.Tests.Services;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Keyboard;
using TestBucket.Localization;

namespace TestBucket.Components.Tests.TestSuites.Commands;

internal class NewFolderCommand : ICommand
{
    private readonly AppNavigationManager _appNavigationManager;
    private readonly TestBrowser _browser;
    private readonly IStringLocalizer<SharedStrings> _loc;

    public int SortOrder => 10;
    public string? Folder => _loc["add"];
    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.TestSuite;
    public PermissionLevel? RequiredLevel => PermissionLevel.ReadWrite;
    public bool Enabled =>
        _appNavigationManager.State.SelectedProject is not null && 
        (
            _appNavigationManager.State.SelectedTestSuite is not null ||
            _appNavigationManager.State.IsTestRepositorySelected ||
            _appNavigationManager.State.IsTestLabSelected ||
            _appNavigationManager.State.SelectedTestLabFolder is not null ||
            _appNavigationManager.State.SelectedTestRepositoryFolder is not null
        );

    public string Id => "new-folder";
    public string Name => _loc["new-folder"];
    public string Description => _loc["new-folder-description"];
    public KeyboardBinding? DefaultKeyboardBinding => new KeyboardBinding() { CommandId = Id, Key = "F7", ModifierKeys = ModifierKey.None };
    public string? Icon => Icons.Material.Filled.CreateNewFolder;
    public string[] ContextMenuTypes => ["TestSuite", "TestSuiteFolder"];
    public NewFolderCommand(AppNavigationManager appNavigationManager, TestBrowser browser, IStringLocalizer<SharedStrings> loc)
    {
        _appNavigationManager = appNavigationManager;
        _browser = browser;
        _loc = loc;
    }

    public async ValueTask ExecuteAsync(ClaimsPrincipal principal)
    {
        if(!Enabled)
        {
            return;
        }
        var project = _appNavigationManager.State.SelectedProject;
        if(project is null)
        {
            return;
        }

        if (_appNavigationManager.State.SelectedTestRepositoryFolder is not null)
        {
            TestRepositoryFolder? parent = _appNavigationManager.State.SelectedTestRepositoryFolder;
            await _browser.AddTestRepositoryFolderAsync(project.Id, parent?.Id);
            return;
        }
        if (_appNavigationManager.State.IsTestRepositorySelected)
        {
            TestRepositoryFolder? parent = null;
            await _browser.AddTestRepositoryFolderAsync(project.Id, parent?.Id);
            return;
        }
        if (_appNavigationManager.State.SelectedTestLabFolder is not null)
        {
            TestLabFolder? parent = _appNavigationManager.State.SelectedTestLabFolder;
            await _browser.AddTestLabFolderAsync(project.Id, parent?.Id);
            return;
        }
        if (_appNavigationManager.State.IsTestLabSelected)
        {
            TestLabFolder? parent = null;
            await _browser.AddTestLabFolderAsync(project.Id, parent?.Id);
            return;
        }
        var suite = _appNavigationManager.State.SelectedTestSuite;
        if (suite is not null)
        {
            var projectId = suite.TestProjectId;
            if (projectId is null)
            {
                return;
            }
            await _browser.AddTestSuiteFolderAsync(projectId.Value, suite.Id, _appNavigationManager.State.SelectedTestSuiteFolder?.Id);
        }
    }
}
