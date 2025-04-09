using System.Collections.Generic;

using Microsoft.CodeAnalysis;

using TestBucket.Components.Shared.Tree;
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

    public async Task<RequirementTestLink[]> GetLinksForTestAsync(TestCase test)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _requirementManager.GetLinksForTestAsync(principal, test);
    }
    public async Task<RequirementTestLink[]> GetLinksForRequirementAsync(Requirement requirement)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _requirementManager.GetLinksForRequirementAsync(principal, requirement);
    }

    public async Task DeleteRequirementLinkAsync(RequirementTestLink link)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        await _requirementManager.DeleteRequirementLinkAsync(principal, link);
    }

    public async Task<RequirementSpecification?> GetRequirementSpecificationByIdAsync(long id)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _requirementManager.GetRequirementSpecificationByIdAsync(principal, id);
    }
    public async Task<Requirement?> GetRequirementByIdAsync(long id)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _requirementManager.GetRequirementByIdAsync(principal, id);
    }

    public async Task<List<TreeNode<BrowserItem>>> SearchAsync(long? teamId, long? projectId, string searchText)
    {
        var specs = await _requirementEditorService.GetRequirementSpecificationsAsync(teamId, projectId);
        var specificationNodes = specs.Items.Select(x => CreateSpecificationNode(x)).ToList();

        var specificationNode = new TreeNode<BrowserItem>
        {
            Text = "Specifications",
            Children = specificationNodes,
            Expanded = true,
            Icon = Icons.Material.Outlined.FolderOpen,
        };

        var rootItems = new List<TreeNode<BrowserItem>>
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
                var requirementNode = CreateRequirementNode(requirement);
                if (specNode.Children is null)
                {
                    specNode.Children = [requirementNode];
                }
                else
                {
                    specNode.Children = [.. specNode.Children, requirementNode];
                }
                specNode.Expanded = true;
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

    /// <summary>
    /// Browsing requirement structure / folders..
    /// </summary>
    /// <param name="teamId"></param>
    /// <param name="projectId"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public async Task<List<TreeNode<BrowserItem>>> BrowseAsync(long? teamId, long? projectId, BrowserItem? parent)
    {
        if (parent is not null)
        {
            List<TreeNode<BrowserItem>> items = [];
            var principal = await GetUserClaimsPrincipalAsync();

            if (parent.Requirement is not null)
            {
                return [];
            }
            if (parent.RequirementFolder is not null)
            {
                var query = new SearchRequirementQuery
                {
                    RequirementSpecificationId = parent.RequirementFolder.RequirementSpecificationId,
                    FolderId = parent.RequirementFolder.Id,
                    CompareFolder = true,
                    Offset = 0,
                    Count = 1_000
                };

                // Add folders
                var folders = await _requirementManager.SearchRequirementFoldersAsync(principal, query);
                items.AddRange(folders.Select(x => CreateTreeNodeFromFolder(x)));

                // Add requirements
                PagedResult<Requirement> result = await _requirementManager.SearchRequirementsAsync(principal, query);
                items.AddRange(result.Items.Select(x => CreateRequirementNode(x)));

                return items;
            }
            if (parent.RequirementSpecification is not null)
            {
                var query = new SearchRequirementQuery
                {
                    FolderId = null,
                    CompareFolder = true,
                    RequirementSpecificationId = parent.RequirementSpecification.Id,
                    Offset = 0,
                    Count = 1_000
                };

                // Add folders
                var folders = await _requirementManager.SearchRequirementFoldersAsync(principal, query);
                items.AddRange(folders.Select(x => CreateTreeNodeFromFolder(x)));

                // Add requirements
                PagedResult <Requirement> result = await _requirementManager.SearchRequirementsAsync(principal, query);
                items.AddRange(result.Items.Select(x=>CreateRequirementNode(x)));

                return items;
            }
        }
        return await BrowseRootAsync(teamId, projectId);
    }

    public TreeNode<BrowserItem> CreateTreeNodeFromFolder(RequirementSpecificationFolder x)
    {
        return new TreeNode<BrowserItem>
        {
            Value = new BrowserItem { RequirementFolder = x, Color = x.Color },
            Text = x.Name,
            Icon = x.Icon ?? Icons.Material.Outlined.Folder,
            Expandable = true,
            Children = null,
        };
    }

    public TreeNode<BrowserItem> CreateRequirementNode(Requirement requirement)
    {
        return new TreeNode<BrowserItem>
        {
            Value = new BrowserItem { Requirement = requirement },
            Text = requirement.Name,
            Icon = Icons.Material.Filled.FactCheck,
            Children = null,
            Expandable = false
        };
    }
    public TreeNode<BrowserItem> CreateSpecificationNode(RequirementSpecification specification)
    {
        return new TreeNode<BrowserItem>
        {
            Expandable = true,
            Expanded = false,
            Value = new BrowserItem { RequirementSpecification = specification, Color = specification.Color },
            Text = specification.Name,
            Icon = specification.Icon ?? Icons.Material.Outlined.Article,
            Children = null,
        };
    }

    private async Task<List<TreeNode<BrowserItem>>> BrowseRootAsync(long? teamId, long? projectId)
    {
        var specs = await _requirementEditorService.GetRequirementSpecificationsAsync(teamId, projectId);
        var specificationNodes = specs.Items.Select(x =>CreateSpecificationNode(x)).ToList();

        return new List<TreeNode<BrowserItem>>
        {
            new TreeNode<BrowserItem>
            {
                Text = "Specifications",
                Children = specificationNodes,
                Expanded = true,
                Value = new BrowserItem() { VirtualFolderName = "Specifications" },
                Icon = Icons.Material.Outlined.FolderOpen,
            }
        };
    }
}
