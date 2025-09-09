
using Plotly.Blazor.Traces.SankeyLib;

using TestBucket.CodeCoverage;
using TestBucket.CodeCoverage.Models;
using TestBucket.Components.Shared.Tree;
using TestBucket.Domain;

namespace TestBucket.Components.Code.Controls.CodeCoverage;

public class PackageCodeCoverageStructureBuilder : BaseCodeStructureBuilder
{
    public override IReadOnlyList<TreeNode<CodeEntity>> Build(CodeCoverageReport report)
    {
        var list = new List<TreeNode<CodeEntity>>();
        foreach (var node in report.Packages)
        {
            list.Add(CreateNode(node));
        }
        return list;
    }
}
