using TestBucket.Contracts.Fields;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Fields.Defaults;
internal class DefaultFieldAssigner
{
    public static void AssignDefaultFields(TestCase testCase, IEnumerable<FieldDefinition> fieldDefinitions)
    {
        testCase.TestCaseFields ??= new();
        foreach (var fieldDefinition in fieldDefinitions.Where(x => (x.Target & FieldTarget.TestCase) == FieldTarget.TestCase))
        {
            if (string.IsNullOrEmpty(fieldDefinition.DefaultValue))
            {
                continue;
            }
            if (testCase.TestCaseFields.Any(x => x.FieldDefinitionId == fieldDefinition.Id))
            {
                continue; // Already assigned
            }
            var testCaseField = new TestCaseField
            {
                TenantId = testCase.TenantId,
                FieldDefinitionId = fieldDefinition.Id,
                TestCaseId = testCase.Id,
            };
            if (FieldValueConverter.TryAssignValue(fieldDefinition, testCaseField, [fieldDefinition.DefaultValue]))
            {
                testCase.TestCaseFields.Add(testCaseField);
            }
        }
    }

    public static void AssignDefaultFields(LocalIssue issue, IEnumerable<FieldDefinition> fieldDefinitions)
    {
        issue.IssueFields ??= new();
        foreach (var fieldDefinition in fieldDefinitions.Where(x => (x.Target & FieldTarget.Issue) == FieldTarget.Issue))
        {
            if (string.IsNullOrEmpty(fieldDefinition.DefaultValue))
            {
                continue;
            }
            if (issue.IssueFields.Any(x => x.FieldDefinitionId == fieldDefinition.Id))
            {
                continue; // Already assigned
            }
            var field = new IssueField
            {
                TenantId = issue.TenantId,
                FieldDefinitionId = fieldDefinition.Id,
                LocalIssueId = issue.Id,
            };
            if (FieldValueConverter.TryAssignValue(fieldDefinition, field, [fieldDefinition.DefaultValue]))
            {
                issue.IssueFields.Add(field);
            }
        }
    }
    public static void AssignDefaultFields(Requirement requirement, IEnumerable<FieldDefinition> fieldDefinitions)
    {
        requirement.RequirementFields ??= new();
        foreach (var fieldDefinition in fieldDefinitions.Where(x => (x.Target & FieldTarget.Requirement) == FieldTarget.Requirement))
        {
            if (string.IsNullOrEmpty(fieldDefinition.DefaultValue))
            {
                continue;
            }
            if (requirement.RequirementFields.Any(x => x.FieldDefinitionId == fieldDefinition.Id))
            {
                continue; // Already assigned
            }
            var field = new RequirementField
            {
                TenantId = requirement.TenantId,
                FieldDefinitionId = fieldDefinition.Id,
                RequirementId = requirement.Id,
            };
            if (FieldValueConverter.TryAssignValue(fieldDefinition, field, [fieldDefinition.DefaultValue]))
            {
                requirement.RequirementFields.Add(field);
            }
        }
    }
}
