using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Code.Models;
public class ProjectArchitectureModel
{
    public Dictionary<string, ArchitecturalComponent> Systems { get; set; } = [];
    public Dictionary<string, ArchitecturalComponent> Components { get; set; } = [];
    public Dictionary<string, ArchitecturalComponent> Layers { get; set; } = [];
    public Dictionary<string, ArchitecturalComponent> Features { get; set; } = [];

}
