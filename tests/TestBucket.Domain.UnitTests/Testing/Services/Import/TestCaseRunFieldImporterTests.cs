using System.Security.Claims;

using NSubstitute;

using TestBucket.Contracts.Fields;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Testing.Services.Import;
using TestBucket.Formats.Dtos;
using TestBucket.Traits.Core;

namespace TestBucket.Domain.UnitTests.Testing.Services.Import;

/// <summary>
/// Unit tests for the <see cref="TestCaseRunFieldImporter"/> class.
/// </summary>
[UnitTest]
[FunctionalTest]
[EnrichedTest]
[Component("Testing")]
[Feature("Import Test Results")]
public class TestCaseRunFieldImporterTests
{
    /// <summary>
    /// Tests that <see cref="TestCaseRunFieldImporter.ImportAsync(TestCaseRunDto, TestCaseRun)"/> correctly assigns values and upserts fields.
    /// </summary>
    [Fact]
    public async Task ImportAsync_ShouldAssignValuesAndUpsertFields()
    {
        // Arrange
        var principal = Substitute.For<ClaimsPrincipal>();
        var fieldManager = Substitute.For<IFieldManager>();
        var fieldDefinitionManager = Substitute.For<IFieldDefinitionManager>();

        var importer = new TestCaseRunFieldImporter(principal, fieldManager, fieldDefinitionManager);

        var source = new TestCaseRunDto
        {
            Traits = new List<TestTrait>
            {
                new TestTrait(TraitType.Feature, "Type1", "Value1"),
                new TestTrait(TraitType.Component, "Type2", "Value2")
            }
        };

        var destination = new TestCaseRun
        {
            TestProjectId = 1,
            TestRunId = 2,
            Id = 3,
            Name = "Test Run"
        };

        var fieldDefinitions = new List<FieldDefinition>
        {
            new FieldDefinition { TraitType = TraitType.Feature, Name = "Feature" },
            new FieldDefinition { TraitType = TraitType.Component, Name = "Component" }
        };

        var fields = new List<TestCaseRunField>
        {
            new TestCaseRunField { FieldDefinition = fieldDefinitions[0], FieldDefinitionId = fieldDefinitions[0].Id, TestCaseRunId = 3, TestRunId = 2 },
            new TestCaseRunField { FieldDefinition = fieldDefinitions[1], FieldDefinitionId = fieldDefinitions[1].Id, TestCaseRunId = 3, TestRunId = 2 }
        };

        fieldDefinitionManager.GetDefinitionsAsync(principal, destination.TestProjectId, FieldTarget.TestCaseRun).Returns(fieldDefinitions);
        fieldManager.GetTestCaseRunFieldsAsync(principal, destination.TestRunId, destination.Id, fieldDefinitions).Returns(fields);

        // Act
        await importer.ImportAsync(source, destination);

        // Assert
        await fieldManager.Received(1).UpsertTestCaseRunFieldAsync(principal, fields[0]);
        await fieldManager.Received(1).UpsertTestCaseRunFieldAsync(principal, fields[1]);
    }
}
