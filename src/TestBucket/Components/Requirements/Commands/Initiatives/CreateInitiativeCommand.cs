using Microsoft.Extensions.Localization;

using TestBucket.Components.Requirements.Services;
using TestBucket.Contracts.Requirements.Types;
using TestBucket.Domain;
using TestBucket.Localization;

namespace TestBucket.Components.Requirements.Commands.Initatives;


internal class CreateInitiativeCommand : CreateWorkItemBaseCommand
{
    public CreateInitiativeCommand(IStringLocalizer
        <RequirementStrings> loc,
        AppNavigationManager appNav,
        RequirementEditorController requirementEditor,
        IDialogService dialogService,
        RequirementBrowser browser) : base(RequirementTypes.Initiative, loc, appNav, requirementEditor, dialogService, browser)
    {
    }
    public override string? Icon => TbIcons.BoldDuoTone.Initiative;
}

