

namespace TestBucket.Domain.UnitTests.Fields.Specifications
{
    [UnitTest]
    [EnrichedTest]
    public class FilterByTenantSpecificationTests
    {
        [Fact]
        public void IsMatch_WithFieldDefinitionsMatch_TrueReturned()
        {
            var specification = new FilterByTenant<FieldDefinition>("tenant1");
            var result = specification.IsMatch(new FieldDefinition { Name = "a", TenantId = "tenant1" });

            Assert.True(result);
        }
        [Fact]
        public void IsMatch_FieldDefinitionNoMatch_FalseReturned()
        {
            var specification = new FilterByTenant<FieldDefinition>("tenant1");
            var result = specification.IsMatch(new FieldDefinition { Name = "a", TenantId = "tenant2" });

            Assert.False(result);
        }

        [Fact]
        public void IsMatch_WithTestSuiteMatch_TrueReturned()
        {
            var specification = new FilterByTenant<TestSuite>("tenant1");
            var result = specification.IsMatch(new TestSuite { Name = "a", TenantId = "tenant1" });

            Assert.True(result);
        }
        
        [Fact]
        public void IsMatch_TestSuiteNoMatch_FalseReturned()
        {
            var specification = new FilterByTenant<TestSuite>("tenant1");
            var result = specification.IsMatch(new TestSuite { Name = "a", TenantId = "tenant2" });

            Assert.False(result);
        }

        [Fact]
        public void IsMatch_WithTestCaseMatch_TrueReturned()
        {
            var specification = new FilterByTenant<TestCase>("tenant1");
            var result = specification.IsMatch(new TestCase { Name = "a", TenantId = "tenant1" });

            Assert.True(result);
        }
        [Fact]
        public void IsMatch_TestCaseNoMatch_FalseReturned()
        {
            var specification = new FilterByTenant<TestCase>("tenant1");
            var result = specification.IsMatch(new TestCase { Name = "a", TenantId = "tenant2" });

            Assert.False(result);
        }
    }
}
