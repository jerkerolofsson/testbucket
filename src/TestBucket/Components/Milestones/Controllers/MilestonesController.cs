using Microsoft.Extensions.Localization;

using TestBucket.Components.Milestones.Dialogs;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Milestones;
using TestBucket.Localization;

namespace TestBucket.Components.Milestones.Controllers;

/// <summary>
/// Controller for managing milestone-related operations in the UI, such as adding, editing, deleting, and retrieving milestones.
/// </summary>
internal class MilestonesController : TenantBaseService
{
    private readonly AppNavigationManager _appNavigationManager;
    private readonly IMilestoneManager _milestoneManager;
    private readonly IDialogService _dialogService;
    private readonly IStringLocalizer<SharedStrings> _loc;

    /// <summary>
    /// Initializes a new instance of the <see cref="MilestonesController"/> class.
    /// </summary>
    /// <param name="authenticationStateProvider">The authentication state provider.</param>
    /// <param name="appNavigationManager">The application navigation manager.</param>
    /// <param name="milestoneManager">The milestone manager service.</param>
    /// <param name="dialogService">The dialog service for UI dialogs.</param>
    /// <param name="loc">The string localizer for shared strings.</param>
    public MilestonesController(
        AuthenticationStateProvider authenticationStateProvider,
        AppNavigationManager appNavigationManager,
        IMilestoneManager milestoneManager,
        IDialogService dialogService,
        IStringLocalizer<SharedStrings> loc) : base(authenticationStateProvider)
    {
        _appNavigationManager = appNavigationManager;
        _milestoneManager = milestoneManager;
        _dialogService = dialogService;
        _loc = loc;
    }

    /// <summary>
    /// Opens a dialog to add a new milestone. If confirmed, adds the milestone.
    /// </summary>
    public async Task AddMilestoneAsync()
    {
        var parameters = new DialogParameters<EditMilestoneDialog>
        {
            { x => x.Milestone, new Milestone() }
        };
        var dialog = await _dialogService.ShowAsync<EditMilestoneDialog>(null, parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        if (result?.Data is Milestone milestone)
        {
            await AddMilestoneAsync(milestone);
        }
    }

    /// <summary>
    /// Prompts the user to confirm deletion of a milestone and deletes it if confirmed.
    /// </summary>
    /// <param name="milestone">The milestone to delete.</param>
    public async Task DeleteMilestoneAsync(Milestone milestone)
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
            await _milestoneManager.DeleteAsync(principal, milestone);
        }

    }

    /// <summary>
    /// Opens a dialog to edit an existing milestone. If confirmed, updates the milestone.
    /// </summary>
    /// <param name="milestone">The milestone to edit.</param>
    public async Task EditMilestoneAsync(Milestone milestone)
    {
        var parameters = new DialogParameters<EditMilestoneDialog>
        {
            { x => x.Milestone, milestone }
        };
        var dialog = await _dialogService.ShowAsync<EditMilestoneDialog>(null, parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        if (result?.Data is Milestone editedMilestone)
        {
            await UpdateMilestoneAsync(editedMilestone);
        }
    }

    /// <summary>
    /// Updates an existing milestone.
    /// </summary>
    /// <param name="milestone">The milestone to update.</param>
    public async Task UpdateMilestoneAsync(Milestone milestone)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _milestoneManager.UpdateMilestoneAsync(principal, milestone);
    }

    /// <summary>
    /// Adds a new milestone to the selected project.
    /// </summary>
    /// <param name="milestone">The milestone to add.</param>
    public async Task AddMilestoneAsync(Milestone milestone)
    {
        milestone.TestProjectId ??= _appNavigationManager.State.SelectedProject?.Id;
        var principal = await GetUserClaimsPrincipalAsync();
        await _milestoneManager.AddMilestoneAsync(principal, milestone);
    }

    /// <summary>
    /// Retrieves the list of milestones for the specified or currently selected project.
    /// </summary>
    /// <param name="project">The project to retrieve milestones for. If null, uses the currently selected project.</param>
    /// <returns>A read-only list of milestones.</returns>
    public async Task<IReadOnlyList<Milestone>> GetMilestonesAsync(TestProject? project)
    {
        project ??= _appNavigationManager.State.SelectedProject;
        if (project is null)
        {
            return [];
        }
        var principal = await GetUserClaimsPrincipalAsync();
        return await _milestoneManager.GetMilestonesAsync(principal, project.Id);
    }

    /// <summary>
    /// Retrieves the list of open milestones for the specified or currently selected project.
    /// </summary>
    /// <param name="project">The project to retrieve milestones for. If null, uses the currently selected project.</param>
    /// <returns>A read-only list of milestones.</returns>
    public async Task<IReadOnlyList<Milestone>> GetOpenMilestonesAsync(TestProject? project)
    {
        var milestones = await GetMilestonesAsync(project);
        return milestones.Where(x => x.State == Contracts.Issues.Models.MilestoneState.Open).ToList();
    }
}