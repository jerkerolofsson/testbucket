namespace TestBucket.Domain.UnitTests.Fields.Specifications
{
    /// <summary>
    /// Contains unit tests for <see cref="FilterByProject{T}"/> specification, which filters entities by their <c>TestProjectId</c> property.
    /// </summary>
    [EnrichedTest]
    [UnitTest]
    public class FilterByProjectSpecificationTests
    {
        /// <summary>
        /// Verifies that FilterByProject.IsMatch returns <c>true</c> when the <c>TestProjectId</c> of a <see cref="FieldDefinition"/> matches the filter.
        /// </summary>
        [Fact]
        public void IsMatch_WithFieldDefinitionsMatch_TrueReturned()
        {
            var specification = new FilterByProject<FieldDefinition>(1);
            var result = specification.IsMatch(new FieldDefinition { Name = "a", TestProjectId = 1 });

            Assert.True(result);
        }

        /// <summary>
        /// Verifies that FilterByProject.IsMatch returns <c>false</c> when the <c>TestProjectId</c> of a <see cref="FieldDefinition"/> does not match the filter.
        /// </summary>
        [Fact]
        public void IsMatch_FieldDefinitionNoMatch_FalseReturned()
        {
            var specification = new FilterByProject<FieldDefinition>(1);
            var result = specification.IsMatch(new FieldDefinition { Name = "a", TestProjectId = 2 });

            Assert.False(result);
        }

        /// <summary>
        /// Verifies that FilterByProject.IsMatch returns <c>true</c> when the <c>TestProjectId</c> of a <see cref="TestSuite"/> matches the filter.
        /// </summary>
        [Fact]
        public void IsMatch_WithTestSuiteMatch_TrueReturned()
        {
            var specification = new FilterByProject<TestSuite>(1);
            var result = specification.IsMatch(new TestSuite { Name = "a", TestProjectId = 1 });

            Assert.True(result);
        }

        /// <summary>
        /// Verifies that FilterByProject.IsMatch returns <c>false</c> when the <c>TestProjectId</c> of a <see cref="TestSuite"/> does not match the filter.
        /// </summary>
        [Fact]
        public void IsMatch_TestSuiteNoMatch_FalseReturned()
        {
            var specification = new FilterByProject<TestSuite>(1);
            var result = specification.IsMatch(new TestSuite { Name = "a", TestProjectId = 2 });

            Assert.False(result);
        }

        /// <summary>
        /// Verifies that FilterByProject.IsMatch returns <c>true</c> when the <c>TestProjectId</c> of a <see cref="TestCase"/> matches the filter.
        /// </summary>
        [Fact]
        public void IsMatch_WithTestCaseMatch_TrueReturned()
        {
            var specification = new FilterByProject<TestCase>(1);
            var result = specification.IsMatch(new TestCase { Name = "a", TestProjectId = 1 });

            Assert.True(result);
        }

        /// <summary>
        /// Verifies that FilterByProject.IsMatch returns <c>false</c> when the <c>TestProjectId</c> of a <see cref="TestCase"/> does not match the filter.
        /// </summary>
        [Fact]
        public void IsMatch_TestCaseNoMatch_FalseReturned()
        {
            var specification = new FilterByProject<TestCase>(1);
            var result = specification.IsMatch(new TestCase { Name = "a", TestProjectId = 2 });

            Assert.False(result);
        }
    }
}