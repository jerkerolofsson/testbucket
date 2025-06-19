using TestBucket.Contracts.Fields;
using TestBucket.Traits.Core;

namespace TestBucket.Domain.Fields.Helpers;
public class FieldDefinitionTemplates
{
    public static IReadOnlyList<FieldDefinition> Templates = [Browser, Priority, FailureType];

    public static FieldDefinition Browser => new FieldDefinition
    {
        Name = "Browser",
        Trait = "Browser",
        Icon = TbIcons.Filled.Globe,
        Target = FieldTarget.TestCaseRun | FieldTarget.TestRun | FieldTarget.TestCase,
        Inherit = true,
        IsVisible = true,
        TraitType = TraitType.Browser,
        Type = FieldType.SingleSelection,
        Options = ["chromium", "firebox", "webkit"],
        Description = "Web browser used for testing"
    };

    public static FieldDefinition TestCategory => new FieldDefinition
    {
        Name = "Category",
        Trait = "TestCategory",
        TraitType = TraitType.TestCategory,
        IsVisible = true,
        Type = FieldType.SingleSelection,
        IsDefinedBySystem = true,
        Inherit = true,
        ShowDescription = false,
        UseClassifier = false,
        Options = ["Unit", "Integration", "E2E", "API"],
        Target = FieldTarget.TestCase | FieldTarget.TestCaseRun,
        RequiredPermission = PermissionLevel.Write
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
        TraitType = Traits.Core.TraitType.TestPriority,
        Inherit = true,
        IsVisible = true,
        Type = FieldType.SingleSelection,
        Options = ["Low", "Medium", "High"],
        Description = "Test Priority"
    };

}
