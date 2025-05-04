
using TestBucket.Components.Milestones.Dialogs;
using TestBucket.Components.Requirements.Dialogs;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Milestones;

namespace TestBucket.Components.Milestones.Controllers;

internal class MilestonesController : TenantBaseService
{
    private readonly AppNavigationManager _appNavigationManager;
    private readonly IMilestoneManager _milestoneManager;
    private readonly IDialogService _dialogService;

    public MilestonesController(AuthenticationStateProvider authenticationStateProvider, AppNavigationManager appNavigationManager, IMilestoneManager milestoneManager, IDialogService dialogService) : base(authenticationStateProvider)
    {
        _appNavigationManager = appNavigationManager;
        _milestoneManager = milestoneManager;
        _dialogService = dialogService;
    }

    public async Task AddMilestoneAsync()
    {
        var parameters = new DialogParameters<EditMilestoneDialog>
        {
            { x => x.Milestone, new Milestone() }
        };
        var dialog = await _dialogService.ShowAsync<EditMilestoneDialog>(null, parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        if(result?.Data is Milestone milestone)
        {
            await AddMilestoneAsync(milestone);
        }
    }


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

    public async Task UpdateMilestoneAsync(Milestone milestone)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _milestoneManager.UpdateMilestoneAsync(principal, milestone);
    }
    public async Task AddMilestoneAsync(Milestone milestone)
    {
        milestone.TestProjectId ??= _appNavigationManager.State.SelectedProject?.Id;
        var principal = await GetUserClaimsPrincipalAsync();
        await _milestoneManager.AddMilestoneAsync(principal, milestone);
    }

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
}
