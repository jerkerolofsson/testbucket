using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Localization;

using TestBucket.Components.Requirements.Dialogs;
using TestBucket.Components.Shared;
using TestBucket.Components.Shared.Tree;
using TestBucket.Components.Tests.TestSuites.Dialogs;
using TestBucket.Domain;
using TestBucket.Domain.Requirements;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Localization;
namespace TestBucket.Components.Requirements.Services;

internal class RequirementBrowser : TenantBaseService
{
    /// <summary>
    /// Virtual folder for all specifications
    /// </summary>
    public const string FOLDER_SPECIFICATIONS = "Specifications";

    private readonly RequirementEditorController _requirementEditorService;
    private readonly IRequirementManager _requirementManager;
    private readonly IStringLocalizer<RequirementStrings> _loc;
    private readonly IStringLocalizer<CodeStrings> _codeLoc;
    private readonly IDialogService _dialogService;
    private readonly AppNavigationManager _appNavigationManager;

    public RequirementBrowser(AuthenticationStateProvider authenticationStateProvider,
        RequirementEditorController requirementEditorService,
        IRequirementManager requirementManager,
        IStringLocalizer<RequirementStrings> loc,
        IDialogService dialogService,
        AppNavigationManager appNavigationManager,
        IStringLocalizer<CodeStrings> codeLoc) : base(authenticationStateProvider)
    {
        _requirementEditorService = requirementEditorService;
        _requirementManager = requirementManager;
        _loc = loc;
        _dialogService = dialogService;
        _appNavigationManager = appNavigationManager;
        _codeLoc = codeLoc;
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
    public async Task<RequirementSpecificationFolder?> GetRequirementFolderByIdAsync(long id)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _requirementManager.GetRequirementFolderByIdAsync(principal, id);
    }
    public async Task<IReadOnlyList<Requirement>> GetRequirementsByAncestorFolderIdAsync(long folderId, int offset, int count)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _requirementManager.GetRequirementsByAncestorFolderIdAsync(principal, folderId, offset, count);
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


    public async Task<PagedResult<Requirement>> SearchRequirementsInSpecificationAsync(long? specificationId, int offset, int count)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        var query = new SearchRequirementQuery { RequirementSpecificationId = specificationId, Offset = offset, Count = count, CompareFolder = false };
        return await _requirementManager.SearchRequirementsAsync(principal, query);
    }

    public async Task<List<TreeNode<BrowserItem>>> SearchAsync(long? teamId, long? projectId, string searchText)
    {
        var specs = await _requirementEditorService.GetRequirementSpecificationsAsync(teamId, projectId);
        var specificationNodes = specs.Items.Select(x => CreateSpecificationNode(x)).ToList();

        TreeNode<BrowserItem> specificationNode = CreateSpecificationRootNode(specificationNodes);

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

        foreach (var requirement in result.Items)
        {
            // Find specification node
            var specNode = rootItems[0].Children!.Where(x => x.Value?.RequirementSpecification?.Id == requirement.RequirementSpecificationId).FirstOrDefault();
            if (specNode is not null)
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
        foreach (var item in specificationNodes.ToList())
        {
            if (item.Children is null || item.Children.Count == 0)
            {
                specificationNodes.Remove(item);
            }
        }
        specificationNode.Children = specificationNodes;



        return rootItems;
    }

    private TreeNode<BrowserItem> CreateSpecificationRootNode(List<TreeNode<BrowserItem>> specificationNodes)
    {
        return new TreeNode<BrowserItem>
        {
            Text = _loc["requirement-specifications"],
            Children = specificationNodes,
            Expanded = true,
            Icon = TbIcons.BoldDuoTone.Database,
            Value = new BrowserItem
            {
                VirtualFolderName = FOLDER_SPECIFICATIONS
            }
        };
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
                var folderQuery = new SearchRequirementFolderQuery
                {
                    RequirementSpecificationId = parent.RequirementFolder.RequirementSpecificationId,
                    FolderId = parent.RequirementFolder.Id,
                    CompareFolder = true,
                    Offset = 0,
                    Count = 1_000
                };
                var requirementQuery = new SearchRequirementQuery
                {
                    RequirementSpecificationId = parent.RequirementFolder.RequirementSpecificationId,
                    FolderId = parent.RequirementFolder.Id,
                    CompareFolder = true,
                    Offset = 0,
                    Count = 1_000
                };

                // Add folders
                var folders = await _requirementManager.SearchRequirementFoldersAsync(principal, folderQuery);
                items.AddRange(folders.Select(x => CreateTreeNodeFromFolder(x)));

                // Add requirements
                PagedResult<Requirement> result = await _requirementManager.SearchRequirementsAsync(principal, requirementQuery);
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
                var folderQuery = new SearchRequirementFolderQuery
                {
                    FolderId = null,
                    CompareFolder = true,
                    RequirementSpecificationId = parent.RequirementSpecification.Id,
                    Offset = 0,
                    Count = 1_000
                };

                // Add folders
                var folders = await _requirementManager.SearchRequirementFoldersAsync(principal, folderQuery);
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
            Icon = RequirementIcons.GetIcon(x),
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
            Icon = RequirementIcons.GetIcon(requirement),
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
            Icon = RequirementIcons.GetIcon(specification),
            Children = null,
        };
    }


    private async Task<List<TreeNode<BrowserItem>>> BrowseRootAsync(long? teamId, long? projectId)
    {
        var specs = await _requirementEditorService.GetRequirementSpecificationsAsync(teamId, projectId);
        var specificationNodes = specs.Items.Select(x =>CreateSpecificationNode(x)).ToList();

        TreeNode<BrowserItem> specificationNode = CreateSpecificationRootNode(specificationNodes);
        TreeNode<BrowserItem> architecture = new TreeNode<BrowserItem>
        {
            Expandable = true,
            Expanded = true,
            Icon = TbIcons.BoldDuoTone.CodeSquare,
            Text = _loc["subject"],
            Children = new List<TreeNode<BrowserItem>>
            {
                new TreeNode<BrowserItem>
                {
                    Text = _codeLoc["product-architecture"],
                    Expandable = false,
                    Icon = TbIcons.BoldDuoTone.CodeSquare,
                    Value = new BrowserItem
                    {
                        Href = _appNavigationManager.GetCodeArchitectureUrl()
                    }
                },
                new TreeNode<BrowserItem>
                {
                    Text = _codeLoc["features"],
                    Expandable = false,
                    Icon = TbIcons.BoldDuoTone.Epic,
                    Value = new BrowserItem
                    {
                        Href = _appNavigationManager.GetFeaturesUrl()
                    }
                },
                new TreeNode<BrowserItem>
                {
                    Text = _codeLoc["commits"],
                    Expandable = false,
                    Icon = TbIcons.Git.Commit,
                    Value = new BrowserItem
                    {
                        Href = _appNavigationManager.GetCodeCommitsUrl()
                    }
                }

            }
        };
        return [specificationNode, architecture];
    }

    internal async Task AddFolderAsync(long projectId, long specificationId, long? parentFolderId)
    {
        var parameters = new DialogParameters<AddRequirementSpecificationFolderDialog>
        {
            { x => x.ProjectId, projectId },
            { x => x.SpecificationId, specificationId },
            { x => x.ParentFolderId, parentFolderId },
        };
        var dialog = await _dialogService.ShowAsync<AddRequirementSpecificationFolderDialog>(null, parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
    }

    internal async Task SyncWithActiveDocumentAsync(Requirement requirement)
    {
        if (_appNavigationManager.State.RequirementTreeView is not null)
        {
            await _appNavigationManager.State.RequirementTreeView.GoToRequirementAsync(requirement);
        }
    }
}
