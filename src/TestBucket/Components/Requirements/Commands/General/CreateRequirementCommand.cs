using Microsoft.Extensions.Localization;

using TestBucket.Components.Requirements.Services;
using TestBucket.Contracts.Requirements.Types;
using TestBucket.Domain;
using TestBucket.Localization;

namespace TestBucket.Components.Requirements.Commands.General;

internal class CreateRequirementCommand : CreateWorkItemBaseCommand
{
    public CreateRequirementCommand(IStringLocalizer
        <RequirementStrings> loc,
        AppNavigationManager appNav,
        RequirementEditorController requirementEditor,
        IDialogService dialogService,
        RequirementBrowser browser) : base(RequirementTypes.General, loc, appNav, requirementEditor, dialogService, browser)
    {
    }

    public override string? Icon => TbIcons.BoldDuoTone.Medal;

}
