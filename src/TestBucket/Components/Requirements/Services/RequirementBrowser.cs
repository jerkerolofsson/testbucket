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

    public async Task<List<TreeItemData<BrowserItem>>> SearchAsync(long? teamId, long? projectId, string searchText)
    {
        var specs = await _requirementEditorService.GetRequirementSpecificationsAsync(teamId, projectId);
        var specificationNodes = specs.Items.Select(x => CreateSpecificationNode(x)).ToList();

        var specificationNode = new TreeItemData<BrowserItem>
        {
            Text = "Specifications",
            Children = specificationNodes,
            Expanded = true,
            Icon = Icons.Material.Filled.FolderOpen,
        };

        var rootItems = new List<TreeItemData<BrowserItem>>
        {
            specificationNode
        };

        var principal = await GetUserClaimsPrincipalAsync();
        var result = await _requirementManager.SearchRequirementsAsync(principal, new SearchRequirementQuery
        {
            Count = 100,
            Offset = 0,
            ProjectId = projectId,
            TeamId = teamId,
            Text = searchText
        });

        foreach(var requirement in result.Items)
        {
            // Find specification node
            var specNode = rootItems[0].Children!.Where(x => x.Value?.RequirementSpecification?.Id == requirement.RequirementSpecificationId).FirstOrDefault();
            if(specNode is not null)
            {
                specNode.Children ??= [];
                specNode.Expanded = true;
                specNode.Children.Add(CreateRequirementNode(requirement));
            }
        }

        // Remove empty specificati0ons
        foreach(var item in specificationNodes.ToList())
        {
            if(item.Children is null || item.Children.Count == 0)
            {
                specificationNodes.Remove(item);
            }
        }
        specificationNode.Children = specificationNodes;



        return rootItems;
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

                return result.Items.Select(x=>CreateRequirementNode(x)).ToList();
            }
        }
        return await BrowseRootAsync(teamId, projectId);
    }

    public TreeItemData<BrowserItem> CreateRequirementNode(Requirement requirement)
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
            Expandable = true,
            Expanded = false,
            Value = new BrowserItem { RequirementSpecification = specification, Color = specification.Color },
            Text = specification.Name,
            Icon = specification.Icon ?? Icons.Material.Outlined.Article,
            Children = null,
        };
    }

    private async Task<List<TreeItemData<BrowserItem>>> BrowseRootAsync(long? teamId, long? projectId)
    {
        var specs = await _requirementEditorService.GetRequirementSpecificationsAsync(teamId, projectId);
        var specificationNodes = specs.Items.Select(x =>CreateSpecificationNode(x)).ToList();

        return new List<TreeItemData<BrowserItem>>
        {
            new TreeItemData<BrowserItem>
            {
                Text = "Specifications",
                Children = specificationNodes,
                Expanded = true,
                Icon = Icons.Material.Filled.FolderOpen,
            }
        };
    }
}
