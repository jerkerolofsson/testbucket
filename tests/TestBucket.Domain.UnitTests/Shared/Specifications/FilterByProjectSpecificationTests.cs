
namespace TestBucket.Domain.UnitTests.Fields.Specifications
{
    [EnrichedTest]
    [UnitTest]
    public class FilterByProjectSpecificationTests
    {
        [Fact]
        public void IsMatch_WithFieldDefinitionsMatch_TrueReturned()
        {
            var specification = new FilterByProject<FieldDefinition>(1);
            var result = specification.IsMatch(new FieldDefinition { Name = "a", TestProjectId = 1 });

            Assert.True(result);
        }
        [Fact]
        public void IsMatch_FieldDefinitionNoMatch_FalseReturned()
        {
            var specification = new FilterByProject<FieldDefinition>(1);
            var result = specification.IsMatch(new FieldDefinition { Name = "a", TestProjectId = 2 });

            Assert.False(result);
        }

        [Fact]
        public void IsMatch_WithTestSuiteMatch_TrueReturned()
        {
            var specification = new FilterByProject<TestSuite>(1);
            var result = specification.IsMatch(new TestSuite { Name = "a", TestProjectId = 1 });

            Assert.True(result);
        }
        [Fact]
        public void IsMatch_TestSuiteNoMatch_FalseReturned()
        {
            var specification = new FilterByProject<TestSuite>(1);
            var result = specification.IsMatch(new TestSuite { Name = "a", TestProjectId = 2 });

            Assert.False(result);
        }

        [Fact]
        public void IsMatch_WithTestCaseMatch_TrueReturned()
        {
            var specification = new FilterByProject<TestCase>(1);
            var result = specification.IsMatch(new TestCase { Name = "a", TestProjectId = 1 });

            Assert.True(result);
        }
        [Fact]
        public void IsMatch_TestCaseNoMatch_FalseReturned()
        {
            var specification = new FilterByProject<TestCase>(1);
            var result = specification.IsMatch(new TestCase { Name = "a", TestProjectId = 2 });

            Assert.False(result);
        }
    }
}
