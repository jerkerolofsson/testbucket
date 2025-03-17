using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Fields.Models;

namespace TestBucket.Domain.Requirements.Models;

[Table("requirement_fields")]
public class RequirementField : FieldValue
{

    // Navigation

    public required long RequirementId { get; set; }
    public Requirement? Requirement { get; set; }
}
