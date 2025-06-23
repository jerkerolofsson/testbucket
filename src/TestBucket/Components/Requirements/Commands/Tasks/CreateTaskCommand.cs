using Microsoft.Extensions.Localization;

using TestBucket.Components.Requirements.Services;
using TestBucket.Domain;
using TestBucket.Localization;

namespace TestBucket.Components.Requirements.Commands.Tasks;

internal class CreateTaskCommand : CreateWorkItemBaseCommand
{
    public CreateTaskCommand(IStringLocalizer
        <RequirementStrings> loc,
        AppNavigationManager appNav,
        RequirementEditorController requirementEditor,
        IDialogService dialogService,
        RequirementBrowser browser) : base("task", loc, appNav, requirementEditor, dialogService, browser)
    {
    }

    public override string? Icon => TbIcons.BoldDuoTone.CheckList;
}
