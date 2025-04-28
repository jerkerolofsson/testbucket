using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Code.Yaml.Models;

namespace TestBucket.Domain.Code.Yaml;
public class ProjectArchitectureModel
{
    public Dictionary<string, ArchitecturalComponent> Systems { get; set; } = [];
    public Dictionary<string, ArchitecturalComponent> Components { get; set; } = [];
    public Dictionary<string, ArchitecturalComponent> Layers { get; set; } = [];
    public Dictionary<string, ArchitecturalComponent> Features { get; set; } = [];

}
