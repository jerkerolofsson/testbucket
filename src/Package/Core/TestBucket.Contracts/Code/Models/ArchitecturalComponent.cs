using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Code.Models;
public class ArchitecturalComponent
{
    public DisplayOptions? Display { get; set; }

    /// <summary>
    /// Description of the component
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Lead Developer
    /// </summary>
    public string? DevLead { get; set; }

    /// <summary>
    /// Lead Tester
    /// </summary>
    public string? TestLead { get; set; }

    /// <summary>
    /// Glob patterns
    /// </summary>
    public List<string>? Paths { get; set; }
}
