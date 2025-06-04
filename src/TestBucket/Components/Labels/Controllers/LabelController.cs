using Microsoft.Extensions.Localization;

using TestBucket.Components.Labels.Dialogs;
using TestBucket.Domain.Labels;
using TestBucket.Localization;
using TestBucket.Domain.Labels.Models;

namespace TestBucket.Components.Labels.Controllers;

/// <summary>
/// Controller for managing label-related operations in the UI, such as adding, editing, deleting, and retrieving labels.
/// </summary>
internal class LabelController : TenantBaseService
{
    private readonly AppNavigationManager _appNavigationManager;
    private readonly ILabelManager _labelManager;
    private readonly IDialogService _dialogService;
    private readonly IStringLocalizer<SharedStrings> _loc;

    /// <summary>
    /// Initializes a new instance of the <see cref="LabelsController"/> class.
    /// </summary>
    /// <param name="authenticationStateProvider">The authentication state provider.</param>
    /// <param name="appNavigationManager">The application navigation manager.</param>
    /// <param name="labelManager">The label manager service.</param>
    /// <param name="dialogService">The dialog service for UI dialogs.</param>
    /// <param name="loc">The string localizer for shared strings.</param>
    public LabelController(
        AuthenticationStateProvider authenticationStateProvider,
        AppNavigationManager appNavigationManager,
        ILabelManager labelManager,
        IDialogService dialogService,
        IStringLocalizer<SharedStrings> loc) : base(authenticationStateProvider)
    {
        _appNavigationManager = appNavigationManager;
        _labelManager = labelManager;
        _dialogService = dialogService;
        _loc = loc;
    }

    /// <summary>
    /// Opens a dialog to add a new label. If confirmed, adds the label.
    /// </summary>
    public async Task AddLabelAsync()
    {
        var parameters = new DialogParameters<EditLabelDialog>
        {
            { x => x.Label, new Label() { ReadOnly = false, Title = "New label" } }
        };
        var dialog = await _dialogService.ShowAsync<EditLabelDialog>(null, parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        if (result?.Data is Label label)
        {
            await AddLabelAsync(label);
        }
    }

    /// <summary>
    /// Prompts the user to confirm deletion of a label and deletes it if confirmed.
    /// </summary>
    /// <param name="label">The label to delete.</param>
    public async Task DeleteLabelAsync(Label label)
    {
        var result = await _dialogService.ShowMessageBox(new MessageBoxOptions
        {
            YesText = _loc["yes"],
            NoText = _loc["no"],
            Title = _loc["confirm-delete-title"],
            MarkupMessage = new MarkupString(_loc["confirm-delete-message"])
        });
        if (result == true)
        {
            var principal = await GetUserClaimsPrincipalAsync();
            await _labelManager.DeleteAsync(principal, label);
        }

    }

    /// <summary>
    /// Opens a dialog to edit an existing label. If confirmed, updates the label.
    /// </summary>
    /// <param name="label">The label to edit.</param>
    public async Task EditLabelAsync(Label label)
    {
        var parameters = new DialogParameters<EditLabelDialog>
        {
            { x => x.Label, label }
        };
        var dialog = await _dialogService.ShowAsync<EditLabelDialog>(null, parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        if (result?.Data is Label editedLabel)
        {
            await UpdateLabelAsync(editedLabel);
        }
    }

    /// <summary>
    /// Updates an existing label.
    /// </summary>
    /// <param name="label">The label to update.</param>
    public async Task UpdateLabelAsync(Label label)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _labelManager.UpdateLabelAsync(principal, label);
    }

    /// <summary>
    /// Adds a new label to the selected project.
    /// </summary>
    /// <param name="label">The label to add.</param>
    public async Task AddLabelAsync(Label label)
    {
        label.TestProjectId ??= _appNavigationManager.State.SelectedProject?.Id;
        var principal = await GetUserClaimsPrincipalAsync();
        await _labelManager.AddLabelAsync(principal, label);
    }

    /// <summary>
    /// Retrieves the list of labels for the specified or currently selected project.
    /// </summary>
    /// <param name="project">The project to retrieve labels for. If null, uses the currently selected project.</param>
    /// <returns>A read-only list of labels.</returns>
    public async Task<IReadOnlyList<Label>> GetLabelsAsync(TestProject? project)
    {
        project ??= _appNavigationManager.State.SelectedProject;
        if (project is null)
        {
            return [];
        }
        var principal = await GetUserClaimsPrincipalAsync();
        return await _labelManager.GetLabelsAsync(principal, project.Id);
    }
}