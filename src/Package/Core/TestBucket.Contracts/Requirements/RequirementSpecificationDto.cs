using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Requirements;

/// <summary>
/// Root level for a collection of requirements
/// </summary>
public class RequirementSpecificationDto : RequirementEntityDto
{
    
    /// <summary>
    /// Icon (SVG)
    /// </summary>
    public string? Icon { get; set; }
    public string? Color { get; set; }
}
