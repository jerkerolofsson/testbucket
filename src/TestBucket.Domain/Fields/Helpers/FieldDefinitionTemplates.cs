namespace TestBucket.Domain.Fields.Helpers;
public class FieldDefinitionTemplates
{
    public static IReadOnlyList<FieldDefinition> Templates = [Browser,TestCategory, Priority, QChar, FailureType, Commit];

    public static FieldDefinition Browser => new FieldDefinition
    {
        Name = "Browser",
        Trait = "Browser",
        Target = FieldTarget.TestCaseRun | FieldTarget.TestRun,
        Inherit = true,
        IsVisible = true,
        Type = FieldType.SingleSelection,
        Options = ["Chrome", "Edge", "Safari"],
        Description = "Web browser used for testing"
    };

    public static FieldDefinition TestCategory => new FieldDefinition
    {
        Name = "Category",
        Trait = "TestCategory",
        Target = FieldTarget.TestCaseRun | FieldTarget.TestCase,
        Inherit = true,
        Type = FieldType.SingleSelection,
        IsVisible = true,
        Options = ["API", "Unit", "Integration", "EndToEnd"],
        Description = "Category of test (unit, integration, api ..)"
    };

    public static FieldDefinition FailureType => new FieldDefinition
    {
        Name = "Failure Type",
        Trait = "FailureType",
        Target = FieldTarget.TestCaseRun,
        Inherit = false,
        IsVisible = true,
        Type = FieldType.String,
        Description = "Test failure type"
    };

    public static FieldDefinition Priority => new FieldDefinition
    {
        Name = "Priority",
        Trait = "Priority",
        Target = FieldTarget.TestCase | FieldTarget.TestCaseRun,
        Inherit = true,
        IsVisible = true,
        Type = FieldType.SingleSelection,
        Options = ["Low", "Medium", "High"],
        Description = "Test Priority"
    };

    public static FieldDefinition Commit => new FieldDefinition
    {
        Name = "Commit",
        Trait = "Commit",
        IsVisible = false,
        Target = FieldTarget.TestRun | FieldTarget.TestCaseRun,
        Inherit = true,
        Type = FieldType.String,
        Description = "Commit SHA1 hash"
    };

    public static FieldDefinition QChar => new FieldDefinition
    {
        Name = "Q-Char",
        Trait = "QualityCharacteristic",
        IsVisible = true,
        Target = FieldTarget.TestCaseRun | FieldTarget.TestCase,
        Inherit = true,
        Type = FieldType.SingleSelection,
        Options = ["Functional Suitability", "Reliability", "Usability", "Performance efficiency", "Compatibility", "Security", "Maintainability", "Portability"],
        Description = "ISO/IEC 25010 quality characteristic related to the type of checks performed in the test"
    };


}
