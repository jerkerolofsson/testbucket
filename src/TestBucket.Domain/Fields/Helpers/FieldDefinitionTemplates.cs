using TestBucket.Contracts.Fields;
using TestBucket.Traits.Core;

namespace TestBucket.Domain.Fields.Helpers;
public class FieldDefinitionTemplates
{
    public static IReadOnlyList<FieldDefinition> Templates = [Branch,  Browser, BrowserVersion, FailureType, HardwareVersion, Priority, Wcag];

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
    public static FieldDefinition BrowserVersion => new FieldDefinition
    {
        Name = "Browser Version",
        Trait = "BrowserVersion",
        Target = FieldTarget.TestCaseRun | FieldTarget.TestRun | FieldTarget.TestCase,
        Inherit = true,
        IsVisible = true,
        TraitType = TraitType.BrowserVersion,
        Type = FieldType.String,
        Description = "Web browser version used for testing"
    };

    public static FieldDefinition Branch => new FieldDefinition
    {
        Name = "Branch",
        Trait = "Branch",
        Target = FieldTarget.TestCaseRun | FieldTarget.TestRun | FieldTarget.TestCase,
        Inherit = true,
        IsVisible = true,
        TraitType = TraitType.Branch,
        Type = FieldType.String,
        Icon = TbIcons.Git.GitBranch,
    };

    public static FieldDefinition HardwareVersion => new FieldDefinition
    {
        Name = "HW Version",
        Trait = "HardwareVersion",
        Target = FieldTarget.TestCaseRun | FieldTarget.TestRun | FieldTarget.TestCase,
        Inherit = true,
        IsVisible = true,
        TraitType = TraitType.HardwareVersion,
        Type = FieldType.String,
    };

    public static FieldDefinition Wcag => new FieldDefinition
    {
        Name = "WCAG Level",
        Trait = "",
        TraitType = TraitType.Custom,
        IsVisible = true,
        Type = FieldType.SingleSelection,
        Inherit = true,
        ShowDescription = false,
        UseClassifier = false,
        Options = ["A", "AA", "AAA"],
        Target = FieldTarget.Requirement,
        RequiredPermission = PermissionLevel.Write,
        Icon = TbIcons.BoldDuoTone.Accessibility,
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
