using TestBucket.Contracts.TestResources;
using TestBucket.Domain.Testing.Execution;

namespace TestBucket.Domain.UnitTests.Testing.Execution;

/// <summary>
/// Unit tests for the <see cref="TestResourceFieldMapper"/> class.
/// </summary>
[FunctionalTest]
[EnrichedTest]
[UnitTest]
[Feature("Android Testing")]
[Component("Testing")]
public class TestResourceFieldMapperTests
{
    /// <summary>
    /// Tests that <see cref="TestResourceFieldMapper.MapResourcesToFields"/> correctly maps a string field.
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-ANDR-001")]
    public void MapResourcesToFields_MapsStringFieldCorrectly()
    {
        // Arrange
        var fields = new List<FieldDefinition>
        {
            new FieldDefinition { Id = 1, Name = "Model", Trait = "SutModel", Type = FieldType.String }
        };

        var testCaseRun = new TestCaseRun { TestRunId = 100, Id = 200, Name = "TestCaseRun1" };

        var resources = new List<TestResourceDto>
        {
            new TestResourceDto
            {
                Owner = "Owner1",
                ResourceId = "Resource1",
                Name = "ResourceName1",
                Types = new[] { "Type1" },
                Variables = new Dictionary<string, string> { { "SutModel", "Volvo 740" } }
            }
        };

        // Act
        var result = TestResourceFieldMapper.MapResourcesToFields(fields, testCaseRun, resources);

        // Assert
        Assert.Single(result);
        Assert.Equal(1, result[0].FieldDefinitionId);
        Assert.Equal("Volvo 740", result[0].StringValue);
    }

    /// <summary>
    /// Verifies that when there are multiple resources but only the second has a matching trait, the value from the second resource is mapped to the field
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-ANDR-001")]
    public void MapResourcesToFields_WithMultipleResources_OnlySecondMatchingTrait_SecondIsUsed()
    {
        // Arrange
        var fields = new List<FieldDefinition>
        {
            new FieldDefinition { Id = 1, Name = "Model", Trait = "SutModel", Type = FieldType.String }
        };

        var testCaseRun = new TestCaseRun { TestRunId = 100, Id = 200, Name = "TestCaseRun1" };

        var resources = new List<TestResourceDto>
        {
            new TestResourceDto
            {
                Owner = "Owner1",
                ResourceId = "Resource1",
                Name = "ResourceName1",
                Types = new[] { "Type1" },
                Variables = new Dictionary<string, string> { { "HasGasPedal", "No" } }
            },
             new TestResourceDto
            {
                Owner = "Owner1",
                ResourceId = "Resource2",
                Name = "ResourceName2",
                Types = new[] { "Type1" },
                Variables = new Dictionary<string, string> { { "SutModel", "Volvo 240" } }
            },
        };

        // Act
        var result = TestResourceFieldMapper.MapResourcesToFields(fields, testCaseRun, resources);

        // Assert
        Assert.Single(result);
        Assert.Equal(1, result[0].FieldDefinitionId);
        Assert.Equal("Volvo 240", result[0].StringValue);
    }

    /// <summary>
    /// Verifies that when there are multiple resources with matching traits, the value from the first resource is mapped to the field
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-ANDR-001")]
    public void MapResourcesToFields_WithMultipleResourcesWithSameField_FirstIsUsed()
    {
        // Arrange
        var fields = new List<FieldDefinition>
        {
            new FieldDefinition { Id = 1, Name = "Model", Trait = "SutModel", Type = FieldType.String }
        };

        var testCaseRun = new TestCaseRun { TestRunId = 100, Id = 200, Name = "TestCaseRun1" };

        var resources = new List<TestResourceDto>
        {
            new TestResourceDto
            {
                Owner = "Owner1",
                ResourceId = "Resource1",
                Name = "ResourceName1",
                Types = new[] { "Type1" },
                Variables = new Dictionary<string, string> { { "SutModel", "Volvo 740" } }
            },
             new TestResourceDto
            {
                Owner = "Owner1",
                ResourceId = "Resource2",
                Name = "ResourceName2",
                Types = new[] { "Type1" },
                Variables = new Dictionary<string, string> { { "SutModel", "Volvo 240" } }
            },
        };

        // Act
        var result = TestResourceFieldMapper.MapResourcesToFields(fields, testCaseRun, resources);

        // Assert
        Assert.Single(result);
        Assert.Equal(1, result[0].FieldDefinitionId);
        Assert.Equal("Volvo 740", result[0].StringValue);
    }

    /// <summary>
    /// Tests that <see cref="TestResourceFieldMapper.MapResourcesToFields"/> correctly maps an integer field.
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-ANDR-001")]
    public void MapResourcesToFields_MapsIntegerFieldCorrectly()
    {
        // Arrange
        var fields = new List<FieldDefinition>
        {
            new FieldDefinition { Id = 2, Name = "IntegerField", Trait = "SoftwareVersion", Type = FieldType.Integer }
        };

        var testCaseRun = new TestCaseRun { TestRunId = 101, Id = 201, Name = "TestCaseRun2" };

        var resources = new List<TestResourceDto>
        {
            new TestResourceDto
            {
                Owner = "Owner2",
                ResourceId = "Resource2",
                Name = "ResourceName2",
                Types = new[] { "Type2" },
                Variables = new Dictionary<string, string> { { "SoftwareVersion", "123" } }
            }
        };

        // Act
        var result = TestResourceFieldMapper.MapResourcesToFields(fields, testCaseRun, resources);

        // Assert
        Assert.Single(result);
        Assert.Equal(2, result[0].FieldDefinitionId);
        Assert.Equal(123L, result[0].LongValue);
    }

    /// <summary>
    /// Tests that <see cref="TestResourceFieldMapper.MapResourcesToFields"/> ignores unmapped fields.
    /// </summary>
    [Fact]
    [CoveredRequirement("TB-ANDR-001")]
    public void MapResourcesToFields_IgnoresUnmappedFields()
    {
        // Arrange
        var fields = new List<FieldDefinition>
        {
            new FieldDefinition { Id = 3, Name = "SW Ver.", Trait = "SoftwareVersion", Type = FieldType.String }
        };

        var testCaseRun = new TestCaseRun { TestRunId = 102, Id = 202, Name = "TestCaseRun3" };

        var resources = new List<TestResourceDto>
        {
            new TestResourceDto
            {
                Owner = "Owner3",
                ResourceId = "Resource3",
                Name = "ResourceName3",
                Types = new[] { "Type3" },
                Variables = new Dictionary<string, string> { { "UnmappedField", "IgnoredValue" } }
            }
        };

        // Act
        var result = TestResourceFieldMapper.MapResourcesToFields(fields, testCaseRun, resources);

        // Assert
        Assert.Empty(result);
    }
}
