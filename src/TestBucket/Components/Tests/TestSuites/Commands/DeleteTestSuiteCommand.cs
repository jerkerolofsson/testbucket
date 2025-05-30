﻿using Microsoft.Extensions.Localization;
using TestBucket.Components.Tests.Services;
using TestBucket.Components.Tests.TestSuites.Services;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Keyboard;
using TestBucket.Localization;

namespace TestBucket.Components.Tests.TestSuites.Commands;

internal class DeleteTestSuiteCommand : ICommand
{
    private readonly AppNavigationManager _appNavigationManager;
    private readonly TestSuiteService _browser;
    private readonly IStringLocalizer<SharedStrings> _loc;
    private readonly IDialogService _dialogService;
    public DeleteTestSuiteCommand(AppNavigationManager appNavigationManager, TestSuiteService browser, IStringLocalizer<SharedStrings> loc, IDialogService dialogService)
    {
        _appNavigationManager = appNavigationManager;
        _browser = browser;
        _loc = loc;
        _dialogService = dialogService;
    }

    public int SortOrder => 90;
    public string? Folder => null;

    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.TestSuite;
    public PermissionLevel? RequiredLevel => PermissionLevel.ReadWrite;
    public bool Enabled => _appNavigationManager.State.SelectedTestSuite is not null &&
        _appNavigationManager.State.SelectedTestRun is null &&
        _appNavigationManager.State.SelectedTestCase is null &&
        _appNavigationManager.State.SelectedTestSuiteFolder is null;
    public string Id => "delete-test-suite";
    public string Name => _loc["delete-test-suite"];
    public string Description => _loc["delete-test-suite-description"];
    public KeyboardBinding? DefaultKeyboardBinding => null;
    public string? Icon => Icons.Material.Filled.Delete;
    public string[] ContextMenuTypes => ["TestSuite"];

    public async ValueTask ExecuteAsync(ClaimsPrincipal principal)
    {
        var suite = _appNavigationManager.State.SelectedTestSuite;
        if (suite is null)
        {
            return;
        }
        var result = await _dialogService.ShowMessageBox(new MessageBoxOptions
        {
            YesText = _loc["yes"],
            NoText = _loc["no"],
            Title = _loc["confirm-delete-title"],
            MarkupMessage = new MarkupString(_loc["confirm-delete-message"])
        });
        if (result == true)
        {
            await _browser.DeleteTestSuiteByIdAsync(suite.Id);
        }
    }
}
