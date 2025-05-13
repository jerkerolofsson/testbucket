using TestBucket.Domain.Requirements.Models;

[Table("requirement_fields")]
public class RequirementField : FieldValue
{

    // Navigation

    public required long RequirementId { get; set; }
    public Requirement? Requirement { get; set; }
}
