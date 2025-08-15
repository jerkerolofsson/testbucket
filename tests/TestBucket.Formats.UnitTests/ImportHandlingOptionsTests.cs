using TestBucket.Formats.JUnit;
using TestBucket.Traits.Xunit;

namespace TestBucket.Formats.UnitTests
{
    /// <summary>
    /// Contains unit tests for the <see cref="ImportHandlingOptions"/> class.
    /// </summary>
    [FunctionalTest]
    [Component("Test Formats")]
    [Feature("Import Test Results")]
    [UnitTest]
    [EnrichedTest]
    public class ImportHandlingOptionsTests
    {
        /// <summary>
        /// Verifies that the default values of <see cref="ImportHandlingOptions"/> are correct.
        /// </summary>
        [Fact]
        public void Default_Values_Are_Correct()
        {
            var options = new ImportHandlingOptions();

            Assert.True(options.CreateFoldersFromClassNamespace);
            Assert.True(options.RemoveClassNameFromTestName);
            Assert.True(options.CreateTestSuiteFromAssemblyName);
            Assert.NotNull(options.Junit);
            Assert.Null(options.TestRunId);
            Assert.Null(options.TestCaseId);
            Assert.Null(options.TestSuiteId);
            Assert.Null(options.AssignTestsToUserName);
        }

        /// <summary>
        /// Verifies that properties of <see cref="ImportHandlingOptions"/> can be set and retrieved.
        /// </summary>
        [Fact]
        public void Can_Set_And_Get_Properties()
        {
            var options = new ImportHandlingOptions
            {
                CreateFoldersFromClassNamespace = false,
                RemoveClassNameFromTestName = false,
                CreateTestSuiteFromAssemblyName = false,
                Junit = new JUnitSerializerOptions { /* set properties if needed */ },
                TestRunId = 123,
                TestCaseId = 456,
                TestSuiteId = 789,
                AssignTestsToUserName = "user"
            };

            Assert.False(options.CreateFoldersFromClassNamespace);
            Assert.False(options.RemoveClassNameFromTestName);
            Assert.False(options.CreateTestSuiteFromAssemblyName);
            Assert.NotNull(options.Junit);
            Assert.Equal(123, options.TestRunId);
            Assert.Equal(456, options.TestCaseId);
            Assert.Equal(789, options.TestSuiteId);
            Assert.Equal("user", options.AssignTestsToUserName);
        }
    }
}