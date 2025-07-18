﻿
using Microsoft.Extensions.Localization;

using TestBucket.Components.Requirements.Dialogs;
using TestBucket.Components.Shared;
using TestBucket.Components.Shared.Fields;
using TestBucket.Components.Shared.Tree;
using TestBucket.Domain;
using TestBucket.Domain.Requirements;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Requirements.Search;
using TestBucket.Domain.Requirements.Specifications.Requirements;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Specifications.TestCases;
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
    private readonly FieldController _fieldController;

    public RequirementBrowser(AuthenticationStateProvider authenticationStateProvider,
        RequirementEditorController requirementEditorService,
        IRequirementManager requirementManager,
        IStringLocalizer<RequirementStrings> loc,
        IDialogService dialogService,
        AppNavigationManager appNavigationManager,
        IStringLocalizer<CodeStrings> codeLoc,
        FieldController fieldController) : base(authenticationStateProvider)
    {
        _requirementEditorService = requirementEditorService;
        _requirementManager = requirementManager;
        _loc = loc;
        _dialogService = dialogService;
        _appNavigationManager = appNavigationManager;
        _codeLoc = codeLoc;
        _fieldController = fieldController;
    }

    public async Task<SearchRequirementQuery?> ShowFilterAsync(SearchRequirementQuery? query)
    {
        query ??= new();

        var parameters = new DialogParameters<EditRequirementFilterDialog>
        {
            { x => x.Query, query },
            { x => x.Project, _appNavigationManager.State.SelectedProject },
        };
        var dialog = await _dialogService.ShowAsync<EditRequirementFilterDialog>(_loc["filter-requirements"], parameters);
        var result = await dialog.Result;
        if (result?.Data is SearchRequirementQuery updatedQuery)
        {
            return updatedQuery;
        }
        return null;
    }

    public async Task<RequirementTestLink[]> GetLinksForTestAsync(TestCase test)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        return await _requirementManager.GetLinksForTestAsync(principal, test);
    }

    /// <summary>
    /// Returns linked tests and other links
    /// </summary>
    /// <param name="requirement"></param>
    /// <returns></returns>
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

    public async Task<PagedResult<Requirement>> SearchAsync(SearchRequirementQuery query, bool semantic = false)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        if(semantic)
        {
            return await _requirementManager.SemanticSearchRequirementsAsync(principal, query);
        }
        return await _requirementManager.SearchRequirementsAsync(principal, query);
    }
    public async Task<List<TreeNode<BrowserItem>>> SearchAsync(long? teamId, long? projectId, string searchText)
    {
        if(projectId is null)
        {
            return [];
        }

        var fields = await _fieldController.GetDefinitionsAsync(projectId.Value, Contracts.Fields.FieldTarget.Requirement);

        var specs = await _requirementEditorService.GetRequirementSpecificationsAsync(teamId, projectId);
        var specificationNodes = specs.Items.Select(x => CreateSpecificationNode(x)).ToList();

        TreeNode<BrowserItem> specificationNode = CreateSpecificationRootNode(specificationNodes);

        var rootItems = new List<TreeNode<BrowserItem>>
        {
            specificationNode
        };

        var principal = await GetUserClaimsPrincipalAsync();
        var query = SearchRequirementQueryParser.Parse(searchText, fields);
        query.Count = 100;
        query.ProjectId = projectId;
        query.CompareFolder = false;
        var result = await _requirementManager.SearchRequirementsAsync(principal,query);
        foreach (var requirement in result.Items)
        {
            await AddToHierarchyAsync(requirement, rootItems);
        }

        // Remove empty specifications
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
            Text = _loc["collections"],
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
    public async Task<List<TreeNode<BrowserItem>>> BrowseAsync(long? teamId, long? projectId, BrowserItem? parent, bool showSearchFolders)
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

                // Add search folders
                if(parent.RequirementSpecification.SearchFolders is not null && showSearchFolders)
                {
                    items.AddRange(parent.RequirementSpecification.SearchFolders.Select(x => new TreeNode<BrowserItem>
                    {
                        Value = new BrowserItem { Href = $"{_appNavigationManager.GetRequirementsSearchUrl()}?q=" + x.Query + " collection-id:" + parent.RequirementSpecification.Id, SearchFolder = x, RequirementSpecification = parent.RequirementSpecification },
                        Text = x.Name,
                        Icon = TbIcons.BoldDuoTone.FolderStar,
                        Expandable = false,
                        Children = null,
                    }));
                }

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
            IconColor = specification.Color,
            Children = null,
        };
    }


    private async Task<List<TreeNode<BrowserItem>>> BrowseRootAsync(long? teamId, long? projectId)
    {
        var principal = await GetUserClaimsPrincipalAsync();

        var result = new List<TreeNode<BrowserItem>>();

        var specs = await _requirementEditorService.GetRequirementSpecificationsAsync(teamId, projectId);
        var specificationNodes = specs.Items.Select(x =>CreateSpecificationNode(x)).ToList();

        // Folder for specifications
        result.Add(CreateSpecificationRootNode(specificationNodes));

        // Search folders
        result.AddRange(RequirementSearchFolders.GetSearchFolders(_loc, _appNavigationManager, principal.Identity?.Name));

        return result;
    }


    private async Task AddToHierarchyAsync(Requirement requirement, List<TreeNode<BrowserItem>> rootItems)
    {
        if (requirement.PathIds is null)
        {
            // We can't show dangling tests..
            return;
        }

        // The root items should already contain the test suites, so we should be able to find it here
        var specificationNode = TreeView<BrowserItem>.FindTreeNode(rootItems, x => x.RequirementSpecification?.Id == requirement.RequirementSpecificationId);
        if (specificationNode is null)
        {
            return;
        }

        TreeNode<BrowserItem> parent = specificationNode;
        specificationNode.Expanded = true;

        // Resolve the parent hierarchy, with test suites and folders
        foreach (var folderId in requirement.PathIds)
        {
            var folderNode = TreeView<BrowserItem>.FindTreeNode(rootItems, x => x.Folder?.Id == folderId);
            if (folderNode is null)
            {
                var items = await BrowseAsync(requirement.TeamId, requirement.TestProjectId, parent.Value, false);
                parent.Children = items;
                folderNode = TreeView<BrowserItem>.FindTreeNode(rootItems, x => x.RequirementFolder?.Id == folderId);
            }
            if (folderNode is null)
            {
                return;
            }
            folderNode.Expanded = true;
            parent = folderNode;
        }

        if (parent is not null)
        {
            var testCaseNode = CreateRequirementNode(requirement);
            if (parent.Children is null)
            {
                parent.Children = [testCaseNode];
            }
            else
            {
                parent.Children = [.. parent.Children, testCaseNode];
            }
        }
    }

    internal async Task SyncWithActiveDocumentAsync(Requirement requirement)
    {
        if (_appNavigationManager.UIState.RequirementTreeView is not null)
        {
            await _appNavigationManager.UIState.RequirementTreeView.GoToRequirementAsync(requirement);
        }
    }

    internal async Task<long[]> GetRequirementIdsFromFolderAsync(RequirementSpecificationFolder folder)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        var tenantId = await GetTenantIdAsync();
        FilterSpecification<Requirement>[] specifications = [new FilterRequirementByAncestorFolder(folder.Id)];
        return await _requirementManager.SearchRequirementIdsAsync(principal, specifications);
    }

    internal async Task<long[]> GetRequirementIdsFromCollectionAsync(RequirementSpecification collection)
    {
        var principal = await GetUserClaimsPrincipalAsync();
        var tenantId = await GetTenantIdAsync();
        FilterSpecification<Requirement>[] specifications = [new FilterRequirementBySpecification(collection.Id)];
        return await _requirementManager.SearchRequirementIdsAsync(principal, specifications);
    }
}
