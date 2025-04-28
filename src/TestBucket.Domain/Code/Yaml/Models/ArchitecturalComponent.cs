using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Code.Yaml.Models;
public class ArchitecturalComponent
{
    public DisplayOptions? Display { get; set; }

    /// <summary>
    /// Glob patterns
    /// </summary>
    public List<string>? Paths { get; set; }
}
