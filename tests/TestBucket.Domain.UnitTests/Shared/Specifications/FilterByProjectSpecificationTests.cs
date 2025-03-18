
namespace TestBucket.Domain.UnitTests.Fields.Specifications
{
    [UnitTest]
    public class FilterByProjectSpecificationTests
    {
        [Test]
        public async Task IsMatch_WithFieldDefinitionsMatch_TrueReturned()
        {
            var specification = new FilterByProject<FieldDefinition>(1);
            var result = specification.IsMatch(new FieldDefinition { Name = "a", TestProjectId = 1 });

            await Assert.That(result).IsTrue();
        }
        [Test]
        public async Task IsMatch_FieldDefinitionNoMatch_FalseReturned()
        {
            var specification = new FilterByProject<FieldDefinition>(1);
            var result = specification.IsMatch(new FieldDefinition { Name = "a", TestProjectId = 2 });

            await Assert.That(result).IsFalse();
        }

        [Test]
        public async Task IsMatch_WithTestSuiteMatch_TrueReturned()
        {
            var specification = new FilterByProject<TestSuite>(1);
            var result = specification.IsMatch(new TestSuite { Name = "a", TestProjectId = 1 });

            await Assert.That(result).IsTrue();
        }
        [Test]
        public async Task IsMatch_TestSuiteNoMatch_FalseReturned()
        {
            var specification = new FilterByProject<TestSuite>(1);
            var result = specification.IsMatch(new TestSuite { Name = "a", TestProjectId = 2 });

            await Assert.That(result).IsFalse();
        }

        [Test]
        public async Task IsMatch_WithTestCaseMatch_TrueReturned()
        {
            var specification = new FilterByProject<TestCase>(1);
            var result = specification.IsMatch(new TestCase { Name = "a", TestProjectId = 1 });

            await Assert.That(result).IsTrue();
        }
        [Test]
        public async Task IsMatch_TestCaseNoMatch_FalseReturned()
        {
            var specification = new FilterByProject<TestCase>(1);
            var result = specification.IsMatch(new TestCase { Name = "a", TestProjectId = 2 });

            await Assert.That(result).IsFalse();
        }
    }
}
