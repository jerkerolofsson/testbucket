using System.Net;

using Microsoft.Extensions.Localization;
using TestBucket.Components.Shared.Tree;
using TestBucket.Domain;
using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Issues.Models;
using TestBucket.Localization;

namespace TestBucket.Components.Requirements.Services;

public class RequirementSearchFolders
{
    public static IEnumerable<TreeNode<BrowserItem>> GetSearchFolders(
        IStringLocalizer loc,
        AppNavigationManager appNavigationManager,
        string? userName)
    {
        yield return new TreeNode<BrowserItem>
        {
            Text = loc["open-epics"],
            Expanded = false,
            Expandable = false,
            Value = new BrowserItem() { Href = $"{appNavigationManager.GetRequirementsSearchUrl()}?q=is:epic open:yes" },
            Icon = TbIcons.BoldDuoTone.FolderStar,
        };

        yield return new TreeNode<BrowserItem>
        {
            Text = loc["closed-epics"],
            Expanded = false,
            Expandable = false,
            Value = new BrowserItem() { Href = $"{appNavigationManager.GetRequirementsSearchUrl()}?q=is:epic open:no" },
            Icon = TbIcons.BoldDuoTone.FolderStar,
        };

        if (userName is not null)
        {
            yield return new TreeNode<BrowserItem>
            {
                Text = loc["my-open-items"],
                Expanded = false,
                Expandable = false,
                Value = new BrowserItem() { Href = $"{appNavigationManager.GetRequirementsSearchUrl()}?q=open:yes assigned-to:{userName}" },
                Icon = TbIcons.BoldDuoTone.FolderStar,
            };
        }
    }
}
