using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Fields;
using TestBucket.Traits.Core;

namespace TestBucket.Domain.Fields.SystemFields;
public static class SystemFieldDefinitions
{
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
        Target = FieldTarget.TestCase |
                    FieldTarget.TestCaseRun |
                    FieldTarget.RequirementSpecificationFolder |
                    FieldTarget.RequirementSpecification |
                    FieldTarget.Requirement |
                    FieldTarget.TestSuite |
                    FieldTarget.TestSuiteFolder
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

    private static readonly FieldDefinition _commit = new()
    {
        Name = "Commit",
        Trait = "commit",
        TraitType = TraitType.Commit,
        Description = "Git commit",
        Icon = TbIcons.Git.Commit,
        IsVisible = true,
        Type = FieldType.String,
        IsDefinedBySystem = true,
        Inherit = true,
        ShowDescription = false,
        UseClassifier = false,
        Options = [],
        Target = FieldTarget.TestRun | FieldTarget.TestCaseRun
    };

    public static FieldDefinition[] Fixed
    {
        get
        {
            return [_feature, _milestone, _commit];
        }
    }
}
