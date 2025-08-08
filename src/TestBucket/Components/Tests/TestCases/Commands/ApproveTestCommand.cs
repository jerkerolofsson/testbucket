using Microsoft.Extensions.Localization;

using TestBucket.Domain;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Keyboard;
using TestBucket.Domain.Testing.TestCases;
using TestBucket.Localization;

namespace TestBucket.Components.Tests.TestCases.Commands;
internal class ApproveTestCommand : ICommand
{
    private readonly IStringLocalizer _loc;
    private readonly AppNavigationManager _appNav;
    private readonly ITestCaseManager _manager;

    public int SortOrder => 20;

    public string? Folder => null;

    public bool Enabled => _appNav.State.SelectedTestCase is not null;
    public string Id => "approve-test";
    public string Name {
        get 
        {
            if(_appNav.State.SelectedTestCase is null)
            {
                return "";
            }
            var message = _loc["approve-test-arg"];
            return string.Format(message, _appNav.State.SelectedTestCase.Name);
        }
    }
    public string Description => "";
    public string? Icon => TbIcons.BoldDuoTone.VerifiedCheck;
    public string[] ContextMenuTypes => [];
    public KeyboardBinding? DefaultKeyboardBinding => null;

    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.Requirement;
    public PermissionLevel? RequiredLevel => PermissionLevel.Approve;

    public ApproveTestCommand(
        IStringLocalizer<RequirementStrings> loc,
        AppNavigationManager appNav,
        ITestCaseManager manager)
    {
        _loc = loc;
        _appNav = appNav;
        _manager = manager;
    }

    public async ValueTask ExecuteAsync(ClaimsPrincipal principal)
    {
        if(_appNav.State.SelectedTestCase is null)
        {
            return;
        }

        var test = _appNav.State.SelectedTestCase;
        await _manager.ApproveTestAsync(principal, test);
    }
}
