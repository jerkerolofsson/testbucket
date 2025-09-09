
using TestBucket.CodeCoverage;
using TestBucket.CodeCoverage.Models;
using TestBucket.Components.Shared.Tree;

namespace TestBucket.Components.Code.Controls.CodeCoverage;

public abstract class BaseCodeStructureBuilder : ICodeCoverageStructureBuilder
{
    public abstract IReadOnlyList<TreeNode<CodeEntity>> Build(CodeCoverageReport report);
    protected virtual TreeNode<CodeEntity> CreateNode(CodeEntity entity)
    {
        return new TreeNode<CodeEntity>
        {
            Expandable = entity.GetChildren().Count > 0,
            Children = null,
            Value = entity,
            Text = entity.GetName(),
            Icon = null
        };
    }

    public IReadOnlyList<TreeNode<CodeEntity>> GetChildren(CodeEntity codeEntity)
    {
        var children = codeEntity.GetChildren();
        var list = new List<TreeNode<CodeEntity>>();
        foreach (var child in children.OrderBy(x=>x.GetName()))
        {
            list.Add(CreateNode(child));
        }
        return list;
    }
}
