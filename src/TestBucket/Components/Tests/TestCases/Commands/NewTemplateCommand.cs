using Microsoft.Extensions.Localization;

using TestBucket.Components.Shared;
using TestBucket.Components.Tests.Controls;
using TestBucket.Components.Tests.TestCases.Services;
using TestBucket.Domain;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Keyboard;
using TestBucket.Localization;

namespace TestBucket.Components.Tests.TestCases.Commands;

internal class NewTemplateCommand : ICommand
{
    private readonly AppNavigationManager _appNavigationManager;
    private readonly TestCaseEditorController _controller;
    private readonly IStringLocalizer<SharedStrings> _loc;
    public int SortOrder => 10;

    public string? Folder => _loc["add"];

    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.TestCase;
    public PermissionLevel? RequiredLevel => PermissionLevel.ReadWrite;
    public bool Enabled => _appNavigationManager.State.SelectedProject is not null;
    public string Id => "new-test-template";
    public string Name => _loc[Id];
    public string Description => _loc["new-test-template-description"];
    public KeyboardBinding? DefaultKeyboardBinding => null;
    public string? Icon => TbIcons.BoldDuoTone.TestTemplate;
    public string[] ContextMenuTypes => ["TestSuite", "TestSuiteFolder", "menu-new"];

    public NewTemplateCommand(AppNavigationManager appNavigationManager, TestCaseEditorController controller, IStringLocalizer<SharedStrings> loc)
    {
        _appNavigationManager = appNavigationManager;
        _controller = controller;
        _loc = loc;
    }

    public async ValueTask ExecuteAsync(ClaimsPrincipal principal)
    {
        if (_appNavigationManager.State.SelectedProject is null)
        {
            return;
        }

        var testCase = await _controller.CreateNewTemplateAsync(_appNavigationManager.State.SelectedProject, _appNavigationManager.State.SelectedTestSuiteFolder, _appNavigationManager.State.SelectedTestSuite?.Id);
        if (testCase is not null)
        {
            _appNavigationManager.NavigateTo(testCase);
        }
    }
}
