using TestBucket.CodeCoverage;
using TestBucket.CodeCoverage.Models;
using TestBucket.Components.Shared.Tree;

namespace TestBucket.Components.Code.Controls.CodeCoverage;

public interface ICodeCoverageStructureBuilder
{
    IReadOnlyList<TreeNode<CodeEntity>> Build(CodeCoverageReport report);
    IReadOnlyList<TreeNode<CodeEntity>> GetChildren(CodeEntity codeEntity);
}
