
using TestBucket.CodeCoverage;
using TestBucket.CodeCoverage.Models;
using TestBucket.Components.Shared.Tree;
using TestBucket.Domain.Code.Models;

namespace TestBucket.Components.Code.Controls.CodeCoverage;

public class CodeCoverageTreeBuilder(CodeCoverageReport Report)
{
    private readonly ICodeCoverageStructureBuilder _default = new PackageCodeCoverageStructureBuilder();

    public IReadOnlyList<TreeNode<CodeEntity>> Build(CodeCoverageStructure structure, IEnumerable<Component> components, IEnumerable<Feature> features)
    {
        return structure switch
        {
            CodeCoverageStructure.Component => new ArchitectureCodeCoverageStructureBuilder<Component>(components).Build(Report),
            CodeCoverageStructure.Feature => new ArchitectureCodeCoverageStructureBuilder<Feature>(features).Build(Report),

            CodeCoverageStructure.Package => _default.Build(Report),
            _ => _default.Build(Report),
        };
    }
    public IReadOnlyCollection<TreeNode<CodeEntity>> GetChildren(CodeCoverageStructure structure, CodeEntity node)
    {
        return _default.GetChildren(node);
    }
}
