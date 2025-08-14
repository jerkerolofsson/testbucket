using TestBucket.Contracts.Fields;
using TestBucket.Domain.Fields.Defaults;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Requirements.Models;

namespace TestBucket.Domain.UnitTests.Fields;

/// <summary>
/// Unit tests for <see cref="DefaultFieldAssigner"/>.
/// Verifies that default fields are correctly assigned to TestCase, LocalIssue, and Requirement entities.
/// </summary>
[Feature("Custom Fields")]
[UnitTest]
[Component("Fields")]
[EnrichedTest]
[SecurityTest]
public class DefaultFieldAssignerTests
{
    /// <summary>
    /// Verifies that default fields are assigned to a <see cref="TestCase"/> when not already present.
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-FIELDS-001")]
    public void AssignDefaultFields_TestCase_AssignsDefaultFields()
    {
        var testCase = new TestCase { Id = 1, Name = "Test" };
        var fieldDef = new FieldDefinition { Id = 10, Name = "Priority", Type = FieldType.String, Target = FieldTarget.TestCase, DefaultValue = "High" };
        DefaultFieldAssigner.AssignDefaultFields(testCase, new[] { fieldDef });
        Assert.NotNull(testCase.TestCaseFields);
        Assert.Single(testCase.TestCaseFields);
        Assert.Equal("High", testCase.TestCaseFields[0].StringValue);
        Assert.Equal(fieldDef.Id, testCase.TestCaseFields[0].FieldDefinitionId);
    }

    /// <summary>
    /// Verifies that default fields are assigned to a <see cref="TestCase"/> when not already present and the field type is Boolean with a true default value.
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-FIELDS-001")]
    public void AssignDefaultFields_WithBooleanTrue_AssignsDefaultFields()
    {
        var testCase = new TestCase { Id = 1, Name = "Test" };
        var fieldDef = new FieldDefinition { Id = 10, Name = "Visible", Type = FieldType.Boolean, Target = FieldTarget.TestCase, DefaultValue = "true" };
        DefaultFieldAssigner.AssignDefaultFields(testCase, new[] { fieldDef });
        Assert.NotNull(testCase.TestCaseFields);
        Assert.Single(testCase.TestCaseFields);
        Assert.Equal(true, testCase.TestCaseFields[0].BooleanValue);
        Assert.Equal(fieldDef.Id, testCase.TestCaseFields[0].FieldDefinitionId);
    }

    /// <summary>
    /// Verifies that default fields are assigned to a <see cref="LocalIssue"/> when not already present.
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-FIELDS-001")]
    public void AssignDefaultFields_LocalIssue_AssignsDefaultFields()
    {
        var issue = new LocalIssue { Id = 2 };
        var fieldDef = new FieldDefinition { Id = 20, Name = "Severity", Type = FieldType.String, Target = FieldTarget.Issue, DefaultValue = "Critical" };
        DefaultFieldAssigner.AssignDefaultFields(issue, new[] { fieldDef });
        Assert.NotNull(issue.IssueFields);
        Assert.Single(issue.IssueFields);
        Assert.Equal("Critical", issue.IssueFields[0].StringValue);
        Assert.Equal(fieldDef.Id, issue.IssueFields[0].FieldDefinitionId);
    }

    /// <summary>
    /// Verifies that default fields are assigned to a <see cref="Requirement"/> when not already present.
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-FIELDS-001")]
    public void AssignDefaultFields_Requirement_AssignsDefaultFields()
    {
        var requirement = new Requirement { Id = 3, Name = "Req" };
        var fieldDef = new FieldDefinition { Id = 30, Name = "Category", Type = FieldType.String, Target = FieldTarget.Requirement, DefaultValue = "Functional" };
        DefaultFieldAssigner.AssignDefaultFields(requirement, new[] { fieldDef });
        Assert.NotNull(requirement.RequirementFields);
        Assert.Single(requirement.RequirementFields);
        Assert.Equal("Functional", requirement.RequirementFields[0].StringValue);
        Assert.Equal(fieldDef.Id, requirement.RequirementFields[0].FieldDefinitionId);
    }

    /// <summary>
    /// Verifies that no duplicate default fields are assigned if already present.
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-FIELDS-001")]
    public void AssignDefaultFields_DoesNotAssignIfAlreadyPresent()
    {
        var testCase = new TestCase { Id = 4, Name = "Test" };
        var fieldDef = new FieldDefinition { Id = 40, Name = "Priority", Type = FieldType.String, Target = FieldTarget.TestCase, DefaultValue = "High" };
        testCase.TestCaseFields = new List<TestCaseField> { new TestCaseField { FieldDefinitionId = 40, TestCaseId = 4, StringValue = "High" } };
        DefaultFieldAssigner.AssignDefaultFields(testCase, new[] { fieldDef });

        Assert.NotNull(testCase.TestCaseFields);
        Assert.Single(testCase.TestCaseFields);
    }

    /// <summary>
    /// Verifies that no default field is assigned if DefaultValue is null or empty for TestCase.
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-FIELDS-001")]
    public void AssignDefaultFields_DoesNotAssignIfDefaultValueIsNullOrEmpty()
    {
        var testCase = new TestCase { Id = 5, Name = "Test" };
        var fieldDef = new FieldDefinition { Id = 50, Name = "Priority", Type = FieldType.String, Target = FieldTarget.TestCase, DefaultValue = null };
        DefaultFieldAssigner.AssignDefaultFields(testCase, new[] { fieldDef });
        Assert.NotNull(testCase.TestCaseFields);
        Assert.Empty(testCase.TestCaseFields);
    }

    /// <summary>
    /// Verifies that a field with a non-matching target is not assigned to a TestCase.
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-FIELDS-001")]
    public void AssignDefaultFields_TestCase_DoesNotAssignIfTargetDoesNotMatch()
    {
        var testCase = new TestCase { Id = 6, Name = "Test" };
        var fieldDef = new FieldDefinition { Id = 60, Name = "IssueOnly", Type = FieldType.String, Target = FieldTarget.Issue, DefaultValue = "Val" };
        DefaultFieldAssigner.AssignDefaultFields(testCase, new[] { fieldDef });
        Assert.NotNull(testCase.TestCaseFields);
        Assert.Empty(testCase.TestCaseFields);
    }

    /// <summary>
    /// Verifies that a field with multiple targets including TestCase is assigned.
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-FIELDS-001")]
    public void AssignDefaultFields_TestCase_AssignsWhenMultipleTargets()
    {
        var testCase = new TestCase { Id = 7, Name = "Test" };
        var fieldDef = new FieldDefinition { Id = 70, Name = "Multi", Type = FieldType.String, Target = FieldTarget.TestCase | FieldTarget.TestSuite, DefaultValue = "MultiVal" };
        DefaultFieldAssigner.AssignDefaultFields(testCase, new[] { fieldDef });
        Assert.NotNull(testCase.TestCaseFields);
        Assert.Single(testCase.TestCaseFields);
        Assert.Equal("MultiVal", testCase.TestCaseFields[0].StringValue);
    }

    /// <summary>
    /// Verifies that no default field is assigned to Issue if DefaultValue is null or empty.
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-FIELDS-001")]
    public void AssignDefaultFields_Issue_DoesNotAssignIfDefaultValueIsNullOrEmpty()
    {
        var issue = new LocalIssue { Id = 8 };
        var fieldDef = new FieldDefinition { Id = 80, Name = "Severity", Type = FieldType.String, Target = FieldTarget.Issue, DefaultValue = string.Empty };
        DefaultFieldAssigner.AssignDefaultFields(issue, new[] { fieldDef });
        Assert.NotNull(issue.IssueFields);
        Assert.Empty(issue.IssueFields);
    }

    /// <summary>
    /// Verifies that no default field is assigned to Requirement if DefaultValue is null or empty.
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-FIELDS-001")]
    public void AssignDefaultFields_Requirement_DoesNotAssignIfDefaultValueIsNullOrEmpty()
    {
        var requirement = new Requirement { Id = 9, Name = "Req" };
        var fieldDef = new FieldDefinition { Id = 90, Name = "Category", Type = FieldType.String, Target = FieldTarget.Requirement, DefaultValue = null };
        DefaultFieldAssigner.AssignDefaultFields(requirement, new[] { fieldDef });
        Assert.NotNull(requirement.RequirementFields);
        Assert.Empty(requirement.RequirementFields);
    }

    /// <summary>
    /// Verifies that mixed field definitions only assign those targeting the entity.
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-FIELDS-001")]
    public void AssignDefaultFields_TestCase_AssignsOnlyMatchingTargetsFromMixedList()
    {
        var testCase = new TestCase { Id = 10, Name = "Test" };
        var defs = new[]
        {
            new FieldDefinition { Id = 100, Name = "TC1", Type = FieldType.String, Target = FieldTarget.TestCase, DefaultValue = "A" },
            new FieldDefinition { Id = 101, Name = "ISS1", Type = FieldType.String, Target = FieldTarget.Issue, DefaultValue = "B" },
            new FieldDefinition { Id = 102, Name = "Both", Type = FieldType.String, Target = FieldTarget.TestCase | FieldTarget.TestSuite, DefaultValue = "C" },
        };
        DefaultFieldAssigner.AssignDefaultFields(testCase, defs);
        Assert.NotNull(testCase.TestCaseFields);
        Assert.Equal(2, testCase.TestCaseFields.Count);
        var values = testCase.TestCaseFields.Select(f => f.StringValue).ToList();
        Assert.Contains("A", values);
        Assert.Contains("C", values);
        Assert.DoesNotContain("B", values);
    }
}
