using Microsoft.Extensions.Localization;

using TestBucket.Components.Requirements.Services;
using TestBucket.Components.Shared.Fields;
using TestBucket.Components.Tests.Dialogs;
using TestBucket.Contracts.Fields;
using TestBucket.Contracts.Requirements.Types;
using TestBucket.Domain;
using TestBucket.Domain.Commands;
using TestBucket.Domain.Keyboard;
using TestBucket.Domain.Progress;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Localization;

namespace TestBucket.Components.Tests.TestSuites.Commands;

internal class BatchTagRequirementsCommand : ICommand
{
    private readonly IStringLocalizer<SharedStrings> _loc;
    private readonly AppNavigationManager _appNavigationManager;
    private readonly RequirementBrowser _browser;
    private readonly IDialogService _dialogService;
    private readonly IProgressManager _progressManager;
    private readonly RequirementEditorController _requirementEditorController;
    private readonly FieldController _fieldController;

    public int SortOrder => 60;
    public string? Folder => null;
    public BatchTagRequirementsCommand(
        IStringLocalizer<SharedStrings> loc,
        AppNavigationManager appNavigationManager,
        RequirementBrowser browser,
        IDialogService dialogService,
        IProgressManager progressManager,
        FieldController fieldController,
        RequirementEditorController requirementEditorController)
    {
        _loc = loc;
        _appNavigationManager = appNavigationManager;
        _browser = browser;
        _dialogService = dialogService;
        _progressManager = progressManager;
        _fieldController = fieldController;
        _requirementEditorController = requirementEditorController;
    }
    public PermissionEntityType? PermissionEntityType => Domain.Identity.Permissions.PermissionEntityType.Requirement;
    public PermissionLevel? RequiredLevel => PermissionLevel.ReadWrite;
    public bool Enabled => _appNavigationManager.State.SelectedRequirementSpecification is not null ||
        _appNavigationManager.State.SelectedRequirementSpecificationFolder is not null ||
        _appNavigationManager.State.SelectedRequirement is not null ||
        _appNavigationManager.State.MultiSelectedRequirements.Count > 0;

    public string Id => "batch-tag-requirements";
    public string Name => _loc["batch-tag-requirements"];
    public string Description => _loc["batch-tag-requirements-description"];
    public KeyboardBinding? DefaultKeyboardBinding => null;
    public string? Icon => TbIcons.BoldDuoTone.Field;
    public string[] ContextMenuTypes => ["RequirementSpecificationFolder", "RequirementSpecification", "Requirement"];

    public async ValueTask ExecuteAsync(ClaimsPrincipal principal)
    {
        var suite = _appNavigationManager.State.SelectedRequirementSpecification;
        var folder = _appNavigationManager.State.SelectedRequirementSpecificationFolder;
        var requirements = _appNavigationManager.State.MultiSelectedRequirements.ToList();

        if(requirements.Count == 0 && _appNavigationManager.State.SelectedRequirement is not null)
        {
            requirements.Add(_appNavigationManager.State.SelectedRequirement);
        }

        if (folder is null && suite is null && requirements.Count == 0)
        {
            return;
        }
        var projectId = folder?.TestProjectId ?? suite?.TestProjectId;
        if(requirements.Count > 0)
        {
            projectId = requirements.First().TestProjectId;
        }
        if (projectId is null)
        {
            return;
        }

        var parameters = new DialogParameters<BatchTagFieldDialog>() 
        { 
            { x => x.ProjectId, projectId }, 
            { x => x.Target, FieldTarget.Requirement }
        };
        var dialog = await _dialogService.ShowAsync<BatchTagFieldDialog>(null, parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        if (result?.Data is FieldValue field)
        {
            await using var progress = _progressManager.CreateProgressTask("Updating requirements..");
            long[] requirementIds = await GetRequirementIdsAsync(suite, folder, requirements);

            await _fieldController.UpdateRequirementFieldsAsync(requirementIds, projectId, new FieldValue[] { field });
        }
        else if (result?.Data is RequirementType requirementType)
        {
            await using var progress = _progressManager.CreateProgressTask("Updating requirements..");
            long[] requirementIds = await GetRequirementIdsAsync(suite, folder, requirements);
            await _requirementEditorController.SetRequirementTypeAsync(requirementIds, requirementType, progress);
        }
    }

    private async Task<long[]> GetRequirementIdsAsync(RequirementSpecification? collection, RequirementSpecificationFolder? folder, IReadOnlyList<Requirement> requirements)
    {
        long[] ids = requirements.Select(x => x.Id).ToArray();
        if (ids.Length == 0)
        {
            if (folder is not null)
            {
                ids = await _browser.GetRequirementIdsFromFolderAsync(folder);
            }
            else if (collection is not null)
            {
                ids = await _browser.GetRequirementIdsFromCollectionAsync(collection);
            }
        }

        return ids;
    }
}
