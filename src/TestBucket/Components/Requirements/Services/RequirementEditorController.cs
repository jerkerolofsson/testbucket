using Mediator;

using Microsoft.Extensions.Localization;

using TestBucket.Components.Tests.TestCases.Dialogs;
using TestBucket.Domain.Files.Models;
using TestBucket.Domain.Requirements;
using TestBucket.Domain.Requirements.Import;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Traceability;
using TestBucket.Domain.Traceability.Models;
using TestBucket.Localization;

namespace TestBucket.Components.Requirements.Services;

internal class RequirementEditorController : TenantBaseService
{
    private readonly IStringLocalizer<SharedStrings> _loc;
    private readonly IStringLocalizer<RequirementStrings> _reqLoc;
    private readonly IRequirementImporter _importer;
    private readonly IRequirementManager _manager;

    private readonly IDialogService _dialogService;
    private readonly IMediator _mediator;

    public RequirementEditorController(
        AuthenticationStateProvider authenticationStateProvider,
        IStringLocalizer<SharedStrings> loc,
        IStringLocalizer<RequirementStrings> reqLoc,
        IRequirementImporter importer,
        IRequirementManager manager,
        IDialogService dialogService,
        IMediator mediator) : base(authenticationStateProvider)
    {
        _loc = loc;
        _reqLoc = reqLoc;
        _importer = importer;
        _manager = manager;
        _dialogService = dialogService;
        _mediator = mediator;
    }

    public async Task<TraceabilityNode> DiscoverTraceabilityAsync(Requirement requirement, int depth)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        principal.ThrowIfNoPermission(PermissionEntityType.Requirement, PermissionLevel.Read);

        return await _mediator.Send(new DiscoverRequirementRelationshipsRequest(principal, requirement, depth));
    }

    public async Task ExtractRequirementsFromSpecificationAsync(RequirementSpecification specification, CancellationToken cancellationToken = default)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        principal.ThrowIfNoPermission(PermissionEntityType.Requirement, PermissionLevel.Write);

        var result = await _dialogService.ShowMessageBox(new MessageBoxOptions
        {
            YesText = _loc["yes"], NoText = _loc["no"],
            Title = "Extract requirements from specification",
            MarkupMessage = new MarkupString("Extracting requirements from the specification will overwrite any existing requirements in this specification. Do you really want to continue?")
        });

        if (result == true)
        {
            var requirements = await _importer.ExtractRequirementsAsync(specification, cancellationToken);

            // Get a copy of all existing links for the specification, as we are
            // deleting requirements, we will re-create these based on the requirement ExternalID property later
            var oldLinks = await _manager.GetRequirementLinksForSpecificationAsync(principal, specification);

            // Delete all old requirements, and folders
            await _manager.DeleteSpecificationRequirementsAndFoldersAsync(principal, specification);

            // Import all new
            foreach (var requirement in requirements)
            {
                cancellationToken.ThrowIfCancellationRequested();

                requirement.TestProjectId = specification.TestProjectId;
                requirement.TeamId = specification.TeamId;
                requirement.RequirementSpecificationId = specification.Id;
                await _manager.AddRequirementAsync(principal, requirement);

                await RestoreRequirementLinks(principal, oldLinks, requirement);
            }
        }
    }

    private async Task RestoreRequirementLinks(ClaimsPrincipal principal, List<RequirementTestLink> oldLinks, Requirement requirement)
    {
        if (requirement.ExternalId is not null)
        {
            var requirementLinks = oldLinks.Where(x => x.RequirementExternalId == requirement.ExternalId).ToList();
            foreach (var link in requirementLinks)
            {
                link.RequirementId = requirement.Id;
                await _manager.AddRequirementLinkAsync(principal, link);
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
        var principal = await GetUserClaimsPrincipalAsync();
        if(!PermissionClaims.HasPermission(principal, PermissionEntityType.Requirement, PermissionLevel.Delete))
        {
            await _dialogService.ShowMessageBox(new MessageBoxOptions
            {
                YesText = _loc["ok"],
                Title = _loc["no-permission-title"],
                MarkupMessage = new MarkupString(_loc["no-permission-message"])
            });
            return;
        }

        var result = await _dialogService.ShowMessageBox(new MessageBoxOptions
        {
            YesText = _loc["yes"], NoText = _loc["no"],
            Title = _reqLoc["confirm-delete-requirement-title"],
            MarkupMessage = new MarkupString(_reqLoc["confirm-delete-requirement-message"])
        });

        if (result == false)
        {
            return;
        }

        await _manager.DeleteRequirementAsync(principal, requirement);

    }
    public async Task DeleteRequirementFolderAsync(RequirementSpecificationFolder folder)
    {
        var result = await _dialogService.ShowMessageBox(new MessageBoxOptions
        {
            YesText = "YES",
            NoText = "NO",
            Title = "Delete folder and all requirements?",
            MarkupMessage = new MarkupString("Do you really want to delete this folder and all requirements?")
        });

        if (result == false)
        {
            return;
        }

        var principal = await GetUserClaimsPrincipalAsync();
        await _manager.DeleteFolderAsync(principal, folder);
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
    }

    public async Task AddRequirementSpecificationAsync(RequirementSpecification specification)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _manager.AddRequirementSpecificationAsync(principal, specification);

    }
    public async Task SaveRequirementSpecificationAsync(RequirementSpecification specification)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _manager.UpdateRequirementSpecificationAsync(principal, specification);
    }

    public async Task SaveRequirementSpecificationFolderAsync(RequirementSpecificationFolder folder)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _manager.UpdateFolderAsync(principal, folder);
    }


    public async Task<RequirementSpecificationFolder> AddRequirementSpecificationFolderAsync(long projectId, long specificationId, long? parent, string name)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        var folder = new RequirementSpecificationFolder() 
        { 
            Name = name, 
            ParentId = parent, 
            RequirementSpecificationId = specificationId, 
            TestProjectId = projectId 
        };

        await _manager.AddFolderAsync(principal, folder);

        return folder;
    }

    public async Task MoveFolderToSpecificationAsync(RequirementSpecificationFolder folder, RequirementSpecification specification)
    {
        var principal = await GetUserClaimsPrincipalAsync();

        folder.Path = "";
        folder.PathIds = null;
        folder.RequirementSpecificationId = specification.Id;
        folder.ParentId = null;
        await _manager.UpdateFolderAsync(principal, folder);
    }

    public async Task MoveFolderToFolderAsync(RequirementSpecificationFolder folder, RequirementSpecificationFolder targetFolder)
    {
        var principal = await GetUserClaimsPrincipalAsync();

        folder.Path = "";
        folder.PathIds = null;
        folder.RequirementSpecificationId = targetFolder.RequirementSpecificationId;
        folder.ParentId = targetFolder.Id;
        await _manager.UpdateFolderAsync(principal, folder);
    }

    public async Task MoveRequirementToSpecificationAsync(Requirement requirement, RequirementSpecification specification)
    {
        requirement.Path = "";
        requirement.PathIds = null;
        requirement.RequirementSpecificationId = specification.Id;
        requirement.RequirementSpecificationFolderId = null;
        await SaveRequirementAsync(requirement);
    }

    public async Task MoveRequirementToFolderAsync(Requirement requirement, RequirementSpecificationFolder folder)
    {
        requirement.Path = "";
        requirement.PathIds = null;
        requirement.RequirementSpecificationId = folder.RequirementSpecificationId;
        requirement.RequirementSpecificationFolderId = folder.Id;
        await SaveRequirementAsync(requirement);
    }

    public async Task AddRequirementAsync(Requirement requirement)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _manager.AddRequirementAsync(principal, requirement);

    }
    public async Task SaveRequirementAsync(Requirement requirement)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _manager.UpdateRequirementAsync(principal, requirement);
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
