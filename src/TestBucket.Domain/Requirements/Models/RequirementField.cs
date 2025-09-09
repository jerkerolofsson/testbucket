using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Requirements.Models;

[Table("requirement_fields")]
[Index(nameof(TenantId), nameof(RequirementId))]
public class RequirementField : FieldValue
{

    // Navigation

    public required long RequirementId { get; set; }
    public Requirement? Requirement { get; set; }
}
