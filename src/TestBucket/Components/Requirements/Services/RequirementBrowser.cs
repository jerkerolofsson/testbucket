using Microsoft.CodeAnalysis;

using TestBucket.Domain.Requirements.Models;
namespace TestBucket.Components.Requirements.Services;

internal class RequirementBrowser
{
    private readonly RequirementEditorService _requirementEditorService;

    public RequirementBrowser(RequirementEditorService requirementEditorService)
    {
        _requirementEditorService = requirementEditorService;
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
                return [];
            }
        }
        return await BrowseRootAsync(teamId, projectId);
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
