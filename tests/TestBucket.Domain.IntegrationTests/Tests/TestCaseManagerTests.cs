using TestBucket.Domain.Testing.TestRuns.Events;

namespace TestBucket.Domain.IntegrationTests.Tests
{
    /// <summary>
    /// Integration tests for the TestCaseManager, verifying test case behaviors and defaults.
    /// </summary>
    [IntegrationTest]
    [EnrichedTest]
    [FunctionalTest]
    public class TestCaseManagerTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        /// <summary>
        /// Verifies that adding an exploratory test case with no session duration sets the default duration to 60 minutes.
        /// </summary>
        [Fact]
        [FunctionalTest]
        [Component("Testing")]

        public async Task AddExploratoryTestCase_WithNoSessionDuration_DefaultsTo60Minutes()
        {
            // Arrange
            var testCase = new TestCase { Name = "Test Case 1", Description = "Charter of test case.", ScriptType = ScriptType.Exploratory };

            // Act
            await Fixture.Tests.AddAsync(testCase);

            // Assert 
            TestCase? test = await Fixture.Tests.GetTestCaseByIdAsync(testCase.Id);
            Assert.NotNull(test);
            Assert.NotNull(test.SessionDuration);
            Assert.Equal(60, test.SessionDuration.Value);
        }

        /// <summary>
        /// Verifies that adding a new test case adds default values for default fields
        /// </summary>
        [Fact]
        [FunctionalTest]
        [CoveredRequirement("TB-FIELDS-001")]
        [Component("Fields")]

        public async Task AddTestCase_DefaultValueFieldAdded()
        {
            // Arrange
            var testCase = new TestCase { Name = "Test Case With Field", Description = "Charter of test case." };

            // Act
            await Fixture.Tests.AddAsync(testCase);

            // Assert 
            TestCase? test = await Fixture.Tests.GetTestCaseByIdAsync(testCase.Id);
            Assert.NotNull(test);
            Assert.NotNull(test.TestCaseFields);

            var qcharField = await Fixture.GetQualityCharacteristicsFieldAsync();
            Assert.Contains(test.TestCaseFields, x => x.FieldDefinitionId == qcharField.Id && x.StringValue == qcharField.DefaultValue);
        }
    }
}
