using DotNet.Globbing;
using TestBucket.Domain.Code.Yaml.Models;

namespace TestBucket.Domain.Code.Yaml;
public class ArchitecturalComponentBrowser(ProjectArchitectureModel Model)
{
    public IEnumerable<NamedArchitecturalComponent> GetSystems()
    {
        if(Model.Systems is null)
        {
            return [];
        }
        List<NamedArchitecturalComponent> systems = new (Model.Systems.Select(x => new NamedArchitecturalComponent(x.Key, x.Value)));
        Sort(systems);
        return systems;
    }

    public IEnumerable<NamedArchitecturalComponent> GetFeatures()
    {
        if (Model.Features is null)
        {
            return [];
        }
        List<NamedArchitecturalComponent> features = new(Model.Features.Select(x => new NamedArchitecturalComponent(x.Key, x.Value)));
        Sort(features);
        return features;
    }

    public IEnumerable<NamedArchitecturalComponent> GetComponentsWithin(ArchitecturalComponent layer)
    {
        List<NamedArchitecturalComponent> result = new();
        if (Model.Components is null)
        {
            return result;
        }
        foreach (var component in Model.Components)
        {
            if (ArePathsOverlapping(layer.Paths, component.Value.Paths))
            {
                result.Add(new NamedArchitecturalComponent(component.Key, component.Value));
            }
        }
        Sort(result);
        return result;
    }

    public IEnumerable<int> GetLayerDisplayTiersWithin(ArchitecturalComponent system)
    {
        return GetLayersWithin(system).Select(x => x.Display?.Sort ?? 0).Distinct().OrderBy(x => x);
    }

    public IEnumerable<NamedArchitecturalComponent> GetLayersWithin(ArchitecturalComponent system, int tier)
    {
        return GetLayersWithin(system).Where(x => (x.Display?.Sort ?? 0) == tier);
    }

    public List<NamedArchitecturalComponent> GetLayersWithin(ArchitecturalComponent system)
    {
        List<NamedArchitecturalComponent> result = new();
        if (Model.Systems is null || Model.Layers is null)
        {
            return result;
        }
        foreach (var layer in Model.Layers)
        {
            if(ArePathsOverlapping(system.Paths, layer.Value.Paths))
            {
                result.Add(new NamedArchitecturalComponent(layer.Key, layer.Value)); 
            }
        }
        Sort(result);
        return result;
    }

    private void Sort(List<NamedArchitecturalComponent> result)
    {
        result.Sort((a, b) =>
        {
            int aOrder = a.Display?.Sort ?? 0;
            int bOrder = b.Display?.Sort ?? 0;
            var diff = aOrder - bOrder;
            if(diff == 0)
            {
                diff = a.Name.CompareTo(b.Name);
            }
            return diff;
        });
    }

    public bool IsFeatureComponent(ArchitecturalComponent? feature, ArchitecturalComponent component)
    {
        if (feature is null)
        {
            return false;
        }
        return ArePathsOverlapping(component.Paths, feature.Paths);
    }

    private bool ArePathsOverlapping(List<string>? pathGlob1, List<string>? pathGlob2)
    {
        if(pathGlob1 is not null && pathGlob2 is not null)
        {
            foreach (var globPattern in pathGlob1)
            {
                if (globPattern.StartsWith('!'))
                {
                    var glob1 = Glob.Parse(globPattern.Substring(1));
                    foreach (var path in pathGlob2)
                    {
                        if (glob1.IsMatch(path))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    var glob1 = Glob.Parse(globPattern);
                    foreach (var path in pathGlob2)
                    {
                        if (glob1.IsMatch(path))
                        {
                            return true;
                        }
                    }
                }
            }

            //foreach (var glob1 in pathGlob1.Where(x=>!x.StartsWith('!')).Select(globPatterns => Glob.Parse(globPatterns)))
            //{
            //    foreach(var path in pathGlob2)
            //    {
            //        if(path.StartsWith('!'))
            //        {
            //            continue;
            //        }

            //        if(glob1.IsMatch(path))
            //        {
            //            return true;
            //        }
            //    }
            //}
        }
        return false;
    }
}
