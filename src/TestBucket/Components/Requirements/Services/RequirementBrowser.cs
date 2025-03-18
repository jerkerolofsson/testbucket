using Microsoft.CodeAnalysis;

using TestBucket.Domain.Requirements;
using TestBucket.Domain.Requirements.Models;
namespace TestBucket.Components.Requirements.Services;

internal class RequirementBrowser : TenantBaseService
{
    private readonly RequirementEditorController _requirementEditorService;
    private readonly IRequirementManager _requirementManager;

    public RequirementBrowser(AuthenticationStateProvider authenticationStateProvider,
        RequirementEditorController requirementEditorService,
        IRequirementManager requirementManager) : base(authenticationStateProvider)
    {
        _requirementEditorService = requirementEditorService;
        _requirementManager = requirementManager;
    }

    public async Task<List<TreeItemData<BrowserItem>>> BrowseAsync(long? teamId, long? projectId, BrowserItem? parent)
    {
        if (parent is not null)
        {
            if (parent.Requirement is not null)
            {
                return [];
            }
            if (parent.RequirementFolder is not null)
            {
                return [];
            }
            if (parent.RequirementSpecification is not null)
            {
                var query = new SearchRequirementQuery
                {
                    RequirementSpecificationId = parent.RequirementSpecification.Id,
                    Offset = 0,
                    Count = 1_000
                };

                var principal = await GetUserClaimsPrincipalAsync();
                PagedResult<Requirement> result = await _requirementManager.SearchRequirementsAsync(principal, query);

                return result.Items.Select(x=>CreateSpecificationNode(x)).ToList();
            }
        }
        return await BrowseRootAsync(teamId, projectId);
    }

    public TreeItemData<BrowserItem> CreateSpecificationNode(Requirement requirement)
    {
        return new TreeItemData<BrowserItem>
        {
            Value = new BrowserItem { Requirement = requirement },
            Text = requirement.Name,
            Icon = Icons.Material.Outlined.Celebration,
            Children = null,
            Expandable = false
        };
    }
    public TreeItemData<BrowserItem> CreateSpecificationNode(RequirementSpecification specification)
    {
        return new TreeItemData<BrowserItem>
        {
            Value = new BrowserItem { RequirementSpecification = specification, Color = specification.Color },
            Text = specification.Name,
            Icon = specification.Icon ?? Icons.Material.Outlined.Article,
            Children = null,
        };
    }

    private async Task<List<TreeItemData<BrowserItem>>> BrowseRootAsync(long? teamId, long? projectId)
    {
        var specs = await _requirementEditorService.GetRequirementSpecificationsAsync(teamId, projectId);
        var suiteItems = specs.Items.Select(x =>CreateSpecificationNode(x)).ToList();

        return new List<TreeItemData<BrowserItem>>
        {
            new TreeItemData<BrowserItem>
            {
                Text = "Specifications",
                Children = suiteItems,
                Expanded = true,
                Icon = Icons.Material.Filled.FolderOpen,
            }
        };
    }
}
