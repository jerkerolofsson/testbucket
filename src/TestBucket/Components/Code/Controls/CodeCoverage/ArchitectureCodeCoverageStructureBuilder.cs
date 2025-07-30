
using DotNet.Globbing;

using Plotly.Blazor.Traces.SankeyLib;

using TestBucket.CodeCoverage;
using TestBucket.CodeCoverage.Models;
using TestBucket.Components.Shared.Tree;
using TestBucket.Domain;
using TestBucket.Domain.Code.Models;

namespace TestBucket.Components.Code.Controls.CodeCoverage;

public class ArchitectureCodeCoverageStructureBuilder<TComponent>
    : BaseCodeStructureBuilder
    where TComponent : AritecturalComponentProjectEntity
{
    private readonly List<TComponent> _components;

    public ArchitectureCodeCoverageStructureBuilder(IEnumerable<TComponent> components)
    {
        _components = components.ToList();
    }


    public override IReadOnlyList<TreeNode<CodeEntity>> Build(CodeCoverageReport report)
    {
        var components = new List<CodeCoveragePackage>();
        var other = new CodeCoveragePackage() { Name = "other" };
        components.Add(other);

        foreach (var node in report.GetClasses(_ => true))
        {
            TComponent? component = FindComponentFromFilename(node.FileName);
            CodeCoveragePackage package = other;
            if (component is not null)
            {
                var componentPackage = components.FirstOrDefault(x => x.Name == component.Name);
                if(componentPackage is null)
                { 
                    componentPackage = new CodeCoveragePackage { Name = component.Name };
                    components.Add(componentPackage);
                }
                package = componentPackage;
            }
            package.AddClass(node);
        }
        return components.Select(x=>CreateNode(x)).ToList();
    }

    private TComponent? FindComponentFromFilename(string fileName)
    {
        foreach(var component in _components)
        {
            foreach(var globPattern in component.GlobPatterns)
            {
                var pattern = Glob.Parse(globPattern);
                if(pattern.IsMatch(fileName))
                {
                    return component;
                }
            }
        }
        return null;
    }
}
