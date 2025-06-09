using System.Net;

using Microsoft.Extensions.Localization;
using TestBucket.Components.Shared.Tree;
using TestBucket.Domain;
using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Issues.Models;
using TestBucket.Localization;

namespace TestBucket.Components.Tests.Services;

public class TestBrowserSearchFolders
{
    public static IEnumerable<TreeNode<BrowserItem>> GetTestCategoryFolders(
        IStringLocalizer<SharedStrings> loc,
        AppNavigationManager appNavigationManager)
    {
        yield return new TreeNode<BrowserItem>
        {
            Text = loc["e2e-tests"],
            Expanded = false,
            Expandable = false,
            Value = new BrowserItem() { Href = $"{appNavigationManager.GetTestCasesUrl()}?q=category:E2E" },
            Icon = TbIcons.BoldOutline.Folder,
        };
        yield return new TreeNode<BrowserItem>
        {
            Text = loc["api-tests"],
            Expanded = false,
            Expandable = false,
            Value = new BrowserItem() { Href = $"{appNavigationManager.GetTestCasesUrl()}?q=category:API" },
            Icon = TbIcons.BoldOutline.Folder,
        };
        yield return new TreeNode<BrowserItem>
        {
            Text = loc["integration-tests"],
            Expanded = false,
            Expandable = false,
            Value = new BrowserItem() { Href = $"{appNavigationManager.GetTestCasesUrl()}?q=category:Integration" },
            Icon = TbIcons.BoldOutline.Folder,
        };
        yield return new TreeNode<BrowserItem>
        {
            Text = loc["unit-tests"],
            Expanded = false,
            Expandable = false,
            Value = new BrowserItem() { Href = $"{appNavigationManager.GetTestCasesUrl()}?q=category:Unit" },
            Icon = TbIcons.BoldOutline.Folder,
        };
    }

    public static IEnumerable<TreeNode<BrowserItem>> GetComponentFolders(
        IStringLocalizer<SharedStrings> loc,
        AppNavigationManager appNavigationManager, 
        IReadOnlyList<Component> components)
    {
        List<TreeNode<BrowserItem>> componentNodes = [];

        if(components.Count == 0)
        {
            yield break;
        }

        foreach(var component in components.OrderBy(x=>x.Name))
        {
            componentNodes.Add(new TreeNode<BrowserItem>
            {
                Text = component.Name,
                Expanded = false,
                Expandable = false,
                Value = new BrowserItem() { Href = $"{appNavigationManager.GetTestCasesUrl()}?q=component:\"{WebUtility.UrlEncode(component.Name)}\"" },
                Icon = TbIcons.BoldOutline.Folder,
            });
        }

        var root = new TreeNode<BrowserItem>
        {
            Text = loc["components"],
            Expanded = false,
            Expandable = true,
            Children = componentNodes,
            Icon = TbIcons.BoldOutline.Folder,
        };

        yield return root;

    }

    public static IEnumerable<TreeNode<BrowserItem>> GetFeatureFolders(
        IStringLocalizer<SharedStrings> loc,
        AppNavigationManager appNavigationManager,
        IReadOnlyList<Feature> features)
    {
        List<TreeNode<BrowserItem>> featureNodes = [];

        if (features.Count == 0)
        {
            yield break;
        }

        foreach (var feature in features.OrderBy(x => x.Name))
        {
            featureNodes.Add(new TreeNode<BrowserItem>
            {
                Text = feature.Name,
                Expanded = false,
                Expandable = false,
                Value = new BrowserItem() { Href = $"{appNavigationManager.GetTestCasesUrl()}?q=feature:\"{WebUtility.UrlEncode(feature.Name)}\"" },
                Icon = TbIcons.BoldOutline.Folder,
            });
        }

        var root = new TreeNode<BrowserItem>
        {
            Text = loc["features"],
            Expanded = false,
            Expandable = true,
            Children = featureNodes,
            Icon = TbIcons.BoldOutline.Folder,
        };

        yield return root;

    }

    public static IEnumerable<TreeNode<BrowserItem>> GetMilestoneFolders(
        IStringLocalizer<SharedStrings> loc,
        AppNavigationManager appNavigationManager,
        IReadOnlyList<Milestone> milestones)
    {
        List<TreeNode<BrowserItem>> componentNodes = [];

        if (milestones.Count == 0)
        {
            yield break;
        }

        foreach (var milestone in milestones.OrderBy(x => x.Title))
        {
            componentNodes.Add(new TreeNode<BrowserItem>
            {
                Text = milestone.Title,
                Expanded = false,
                Expandable = false,
                Value = new BrowserItem() { Href = $"{appNavigationManager.GetTestCasesUrl()}?q=milestone:\"{WebUtility.UrlEncode(milestone.Title)}\"" },
                Icon = TbIcons.BoldOutline.Folder,
            });
        }

        var root = new TreeNode<BrowserItem>
        {
            Text = loc["milestones"],
            Expanded = false,
            Expandable = true,
            Children = componentNodes,
            Icon = TbIcons.BoldOutline.Folder,
        };

        yield return root;

    }
}
