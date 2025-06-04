using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Code.Yaml.Models;
public class NamedArchitecturalComponent : ArchitecturalComponent
{
    public NamedArchitecturalComponent(string name, ArchitecturalComponent parent)
    {
        Name = name;
        Display = parent.Display;
        Paths = parent.Paths;
        Description = parent.Description;
    }

    public string Name { get; }
}
