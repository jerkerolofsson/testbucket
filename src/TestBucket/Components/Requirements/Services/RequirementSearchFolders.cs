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
        AppNavigationManager appNavigationManager)
    {
        yield return new TreeNode<BrowserItem>
        {
            Text = loc["epics"],
            Expanded = false,
            Expandable = false,
            Value = new BrowserItem() { Href = $"{appNavigationManager.GetRequirementsSearchUrl()}?q=is:epic" },
            Icon = TbIcons.BoldDuoTone.FolderStar,
        };
    }
}
