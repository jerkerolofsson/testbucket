﻿using Microsoft.Extensions.Localization;

using TestBucket.Domain;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Keyboard;
using TestBucket.Localization;

namespace TestBucket.Components.Requirements.Commands.Collections;

internal class ImportRequirementsCommand : ICommand
{
    public string Id => "import-requirements";

    public string Name => _reqLoc["import-requirements"];

    public string Description => _reqLoc["import-requirements-description"];

    public bool Enabled => _appNav.State.SelectedProject is not null;

    public int SortOrder => 50;

    public string? Folder => null;

    public KeyboardBinding? DefaultKeyboardBinding => null;
    public string? Icon => TbIcons.BoldDuoTone.Import;
    public string[] ContextMenuTypes => ["menu-requirements", "menu-import"];
    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.RequirementSpecification;
    public PermissionLevel? RequiredLevel => PermissionLevel.Write;
    private readonly IStringLocalizer<SharedStrings> _loc;
    private readonly IStringLocalizer<RequirementStrings> _reqLoc;
    private readonly AppNavigationManager _appNav;

    public ImportRequirementsCommand(
        IStringLocalizer<RequirementStrings> reqLoc,
        AppNavigationManager appNav,
        IStringLocalizer<SharedStrings> loc)
    {
        _reqLoc = reqLoc;
        _appNav = appNav;
        _loc = loc;
    }

    public ValueTask ExecuteAsync(ClaimsPrincipal principal)
    {
        var project = _appNav.State.SelectedProject;
        if(project is null)
        {
            return ValueTask.CompletedTask;
        }

        _appNav.NavigateTo(_appNav.GetImportSpecificationsUrl(), false);
        return ValueTask.CompletedTask;
    }
}
