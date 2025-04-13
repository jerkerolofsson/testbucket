using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Requirements;
public class RequirementDto : RequirementEntityDto
{
    public string? State { get; set; }
    public string? Path { get; set; }
}
