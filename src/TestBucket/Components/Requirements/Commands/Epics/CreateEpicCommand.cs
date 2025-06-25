using Microsoft.Extensions.Localization;

using TestBucket.Components.Requirements.Dialogs;
using TestBucket.Components.Requirements.Services;
using TestBucket.Contracts.Requirements.Types;
using TestBucket.Domain;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Keyboard;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Localization;

namespace TestBucket.Components.Requirements.Commands.Epics;

internal class CreateEpicCommand : CreateWorkItemBaseCommand
{
    public CreateEpicCommand(IStringLocalizer
        <RequirementStrings> loc, 
        AppNavigationManager appNav, 
        RequirementEditorController requirementEditor, 
        IDialogService dialogService, 
        RequirementBrowser browser) : base(RequirementTypes.Epic, loc, appNav, requirementEditor, dialogService, browser)
    {
    }

    public override string? Icon => TbIcons.BoldDuoTone.Epic;

}
