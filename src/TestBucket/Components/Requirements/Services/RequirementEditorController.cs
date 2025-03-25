using TestBucket.Components.Tests.Dialogs;
using TestBucket.Domain.Files.Models;
using TestBucket.Domain.Requirements;
using TestBucket.Domain.Requirements.Import;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Teams.Models;

namespace TestBucket.Components.Requirements.Services;
internal interface IRequirementObserver
{
    Task OnSpecificationCreatedAsync(RequirementSpecification spec);
    Task OnSpecificationDeletedAsync(RequirementSpecification spec);
    Task OnSpecificationSavedAsync(RequirementSpecification spec);
}
internal class RequirementEditorController : TenantBaseService
{
    private readonly List<IRequirementObserver> _requirementObservers = new();
    private readonly IRequirementImporter _importer;
    private readonly IRequirementManager _manager;

    private readonly IDialogService _dialogService;


    public RequirementEditorController(
        AuthenticationStateProvider authenticationStateProvider,
        IRequirementImporter importer,
        IRequirementManager manager,
        IDialogService dialogService) : base(authenticationStateProvider)
    {
        _importer = importer;
        _manager = manager;
        _dialogService = dialogService;
    }

    /// <summary>
    /// Adds an observer
    /// </summary>
    /// <param name="listener"></param>
    public void AddObserver(IRequirementObserver observer) => _requirementObservers.Add(observer);

    /// <summary>
    /// Removes an observer
    /// </summary>
    /// <param name="observer"></param>
    public void RemoveObserver(IRequirementObserver observer) => _requirementObservers.Remove(observer);

    public async Task ExtractRequirementsFromSpecificationAsync(RequirementSpecification specification, CancellationToken cancellationToken = default)
    {
        var principal = await GetUserClaimsPrincipalAsync();

        var result = await _dialogService.ShowMessageBox(new MessageBoxOptions
        {
            YesText = "YES", NoText = "NO",
            Title = "Extract requirements from specification",
            MarkupMessage = new MarkupString("Extracting requirements from the specification will overwrite any existing requirements in this specification. Do you really want to continue?")
        });

        if (result == true)
        {
            var requirements = await _importer.ExtractRequirementsAsync(specification, cancellationToken);

            // Delete all old
            await _manager.DeleteSpecificationRequirementsAndFoldersAsync(principal, specification);

            // Import all new
            foreach (var requirement in requirements)
            {
                cancellationToken.ThrowIfCancellationRequested();

                requirement.TestProjectId = specification.TestProjectId;
                requirement.TeamId = specification.TeamId;
                requirement.RequirementSpecificationId = specification.Id;
                await _manager.AddRequirementAsync(principal, requirement);
            }
        }
    }

    public async Task<RequirementSpecification?> ImportAsync(long? teamId, long? testProjectId, FileResource fileResource)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        var specification = await _importer.ImportFileAsync(principal, teamId, testProjectId, fileResource);

        if (specification is not null)
        {
            await AddRequirementSpecificationAsync(specification);
        }

        return specification;
    }

    public async Task LinkRequirementToTestCaseAsync(Requirement requirement, TestProject? project, Team? team)
    {
        var principal = await GetUserClaimsPrincipalAsync();

        var parameters = new DialogParameters<PickTestCaseDialog>()
        {
            { x => x.Project, project },
            { x => x.Team, team },
        };

        var dialog = await _dialogService.ShowAsync<PickTestCaseDialog>("Select test", parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
        if (result?.Data is TestCase testCase)
        {
            await _manager.AddRequirementLinkAsync(principal, requirement, testCase);
        }
    }


    public async Task LinkRequirementToTestCaseAsync(Requirement requirement, TestCase testCase)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _manager.AddRequirementLinkAsync(principal, requirement, testCase);
    }

    public async Task DeleteRequirementAsync(Requirement requirement)
    {
        var result = await _dialogService.ShowMessageBox(new MessageBoxOptions
        {
            YesText = "YES", NoText = "NO",
            Title = "Delete requirement?",
            MarkupMessage = new MarkupString("Do you really want to delete this requirement?")
        });

        if (result == false)
        {
            return;
        }

        var principal = await GetUserClaimsPrincipalAsync();
        await _manager.DeleteRequirementAsync(principal, requirement);

    }

    public async Task DeleteRequirementSpecificationAsync(RequirementSpecification specification)
    {
        var result = await _dialogService.ShowMessageBox(new MessageBoxOptions
        {
            YesText = "YES",
            NoText = "NO",
            Title = "Delete specification and all requirements?",
            MarkupMessage = new MarkupString("Do you really want to delete this requirement specification and all requirements?")
        });

        if(result == false)
        {
            return;
        }

        var principal = await GetUserClaimsPrincipalAsync();
        await _manager.DeleteRequirementSpecificationAsync(principal, specification);

        foreach (var observer in _requirementObservers)
        {
            await observer.OnSpecificationDeletedAsync(specification);
        }
    }

    public async Task AddRequirementSpecificationAsync(RequirementSpecification specification)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _manager.AddRequirementSpecificationAsync(principal, specification);

        foreach (var observer in _requirementObservers)
        {
            await observer.OnSpecificationCreatedAsync(specification);
        }
    }
    public async Task SaveRequirementSpecificationAsync(RequirementSpecification specification)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _manager.UpdateRequirementSpecificationAsync(principal, specification);

        foreach(var observer in _requirementObservers)
        {
            await observer.OnSpecificationSavedAsync(specification);
        }
    }

    public async Task SaveRequirementAsync(Requirement requirement)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _manager.UpdateRequirementAsync(principal, requirement);

        //foreach (var observer in _requirementObservers)
        //{
        //    await observer.OnReq(specification);
        //}
    }

    public async Task<PagedResult<RequirementSpecification>> GetRequirementSpecificationsAsync(long? teamId, long? projectId, int offset = 0, int count = 100)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _manager.SearchRequirementSpecificationsAsync(principal, new SearchQuery
        {
            TeamId = teamId,
            ProjectId = projectId,
            Offset = offset,
            Count = count
        });
    }


}
