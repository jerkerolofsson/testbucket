using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Fields;
using TestBucket.Domain.Fields.Helpers;
using TestBucket.Traits.Core;

namespace TestBucket.Domain.Fields.SystemFields;

/// <summary>
/// Defines the default fields that are created for a project
/// </summary>
public static class SystemFieldDefinitions
{
    private static readonly FieldDefinition _label = new()
    {
        Name = "Label",
        Trait = "label",
        TraitType = TraitType.Label,
        Icon = TbIcons.IconSaxDuoTone.Label,
        IsVisible = true,
        Type = FieldType.MultiSelection,
        IsDefinedBySystem = true,
        Inherit = true,
        ShowDescription = false,
        UseClassifier = false,
        Options = [],
        DataSourceType = FieldDataSourceType.Labels,
        RequiredPermission = PermissionLevel.Write,
        Target = FieldTarget.Issue | FieldTarget.Requirement
    };

    private static readonly FieldDefinition _feature = new()
    {
        Name = "Feature",
        Trait = "feature",
        TraitType = TraitType.Feature,
        Description = "Related Product Feature",
        IsVisible = true,
        Icon = TbIcons.BoldDuoTone.Epic,
        Type = FieldType.SingleSelection,
        IsDefinedBySystem = true,
        Inherit = true,
        UseClassifier = true,
        Options = [],
        DataSourceType = FieldDataSourceType.Features,
        RequiredPermission = PermissionLevel.Write,
        Target = FieldTarget.TestCase |
                    FieldTarget.TestCaseRun |
                    FieldTarget.RequirementSpecificationFolder |
                    FieldTarget.RequirementSpecification |
                    FieldTarget.Requirement |
                    FieldTarget.TestSuite |
                    FieldTarget.TestSuiteFolder |
                    FieldTarget.Issue
    };

    private static readonly FieldDefinition _component = new()
    {
        Name = "Component",
        Trait = "component",
        TraitType = TraitType.Component,
        Description = "Component/Area/Module",
        IsVisible = true,
        Icon = TbIcons.BoldDuoTone.Components,
        Type = FieldType.SingleSelection,
        IsDefinedBySystem = true,
        Inherit = true,
        UseClassifier = true,
        Options = [],
        DataSourceType = FieldDataSourceType.Components,
        RequiredPermission = PermissionLevel.Write,
        Target = FieldTarget.TestCase |
                FieldTarget.TestCaseRun |
                FieldTarget.RequirementSpecificationFolder |
                FieldTarget.RequirementSpecification |
                FieldTarget.Requirement |
                FieldTarget.Issue
    };

    private static readonly FieldDefinition _milestone = new()
    {
        Name = "Milestone",
        Trait = "milestone",
        TraitType = TraitType.Milestone,
        Description = "Product Milestone",
        Icon = TbIcons.BoldDuoTone.Flag,
        IsVisible = true,
        Type = FieldType.String,
        IsDefinedBySystem = true,
        Inherit = true,
        ShowDescription = false,
        UseClassifier = false,
        Options = [],
        DataSourceType = FieldDataSourceType.Milestones,
        RequiredPermission = PermissionLevel.Write,
        Target = FieldTarget.TestCase |
                    FieldTarget.TestRun |
                    FieldTarget.TestCaseRun |
                    FieldTarget.RequirementSpecificationFolder |
                    FieldTarget.RequirementSpecification |
                    FieldTarget.TestSuite |
                    FieldTarget.Issue |
                    FieldTarget.TestSuiteFolder |
                    FieldTarget.Requirement
    };


    private static readonly FieldDefinition _qualityCharacteristic = new()
    {
        Name = "Quality Characteristic",
        Trait = "QualityCharacteristic",
        TraitType = TraitType.QualityCharacteristic,
        Description = "ISO/IEC 25010 Software Quality Characteristic",
        IsVisible = true,
        Type = FieldType.SingleSelection,
        IsDefinedBySystem = true,
        Inherit = true,
        ShowDescription = true,
        UseClassifier = false,
        Options = 
        [
            "Functional Suitability",
            "Performance Efficiency",
            "Compatibility",
            "Usability",
            "Reliability",
            "Security",
            "Maintainability",
            "Portability"
        ],
        RequiredPermission = PermissionLevel.Write,
        Target = FieldTarget.TestCase |
                    FieldTarget.TestCaseRun |
                    FieldTarget.Issue |
                    FieldTarget.Requirement
    };

    private static readonly FieldDefinition _branch = new()
    {
        Name = "Branch",
        Trait = "branch",
        TraitType = TraitType.Branch,
        Description = "Branch",
        Icon = TbIcons.Git.GitBranch,
        IsVisible = true,
        Type = FieldType.String,
        IsDefinedBySystem = true,
        Inherit = true,
        ShowDescription = false,
        UseClassifier = false,
        Options = [],
        Target = FieldTarget.TestRun | FieldTarget.TestCaseRun,
        RequiredPermission = PermissionLevel.Write
    };

    private static readonly FieldDefinition _commit = new()
    {
        Name = "Commit",
        Trait = "commit",
        TraitType = TraitType.Commit,
        Description = "Git commit",
        Icon = TbIcons.Git.Commit,
        DataSourceType = FieldDataSourceType.Commit,
        IsVisible = true,
        Type = FieldType.String,
        IsDefinedBySystem = true,
        Inherit = true,
        ShowDescription = false,
        UseClassifier = false,
        Options = [],
        Target = FieldTarget.TestRun | FieldTarget.TestCaseRun | FieldTarget.Issue,
        RequiredPermission = PermissionLevel.Write
    };

    private static readonly FieldDefinition _requirementApproval = new()
    {
        Name = "Approved",
        Trait = "approved",
        TraitType = TraitType.Approved,
        Description = "Approved",
        Icon = TbIcons.BoldDuoTone.VerifiedCheck,
        IsVisible = true,
        Type = FieldType.Boolean,
        IsDefinedBySystem = true,
        Inherit = false,
        ShowDescription = false,
        UseClassifier = false,
        Options = [],
        Target = FieldTarget.Requirement,
        RequiredPermission = PermissionLevel.Approve
    };


    private static readonly FieldDefinition _testCategory = FieldDefinitionTemplates.TestCategory;

    public static FieldDefinition[] Fixed
    {
        get
        {
            return [_component,_requirementApproval, _feature, _milestone, _commit, _branch, _label, _testCategory, _qualityCharacteristic];
        }
    }
}
