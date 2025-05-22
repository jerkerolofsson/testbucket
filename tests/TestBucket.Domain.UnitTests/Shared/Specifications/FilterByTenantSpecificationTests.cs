namespace TestBucket.Domain.UnitTests.Fields.Specifications
{
    /// <summary>
    /// Contains unit tests for <see cref="FilterByTenant{T}"/> specification, which filters entities by their <c>TenantId</c> property.
    /// </summary>
    [UnitTest]
    [EnrichedTest]
    public class FilterByTenantSpecificationTests
    {
        /// <summary>
        /// Verifies that <c>FilterByTenant.IsMatch</c> returns <c>true</c> when the <c>TenantId</c> of a <see cref="FieldDefinition"/> matches the filter.
        /// </summary>
        [Fact]
        public void IsMatch_WithFieldDefinitionsMatch_TrueReturned()
        {
            var specification = new FilterByTenant<FieldDefinition>("tenant1");
            var result = specification.IsMatch(new FieldDefinition { Name = "a", TenantId = "tenant1" });

            Assert.True(result);
        }

        /// <summary>
        /// Verifies that <c>FilterByTenant.IsMatch</c> returns <c>false</c> when the <c>TenantId</c> of a <see cref="FieldDefinition"/> does not match the filter.
        /// </summary>
        [Fact]
        public void IsMatch_FieldDefinitionNoMatch_FalseReturned()
        {
            var specification = new FilterByTenant<FieldDefinition>("tenant1");
            var result = specification.IsMatch(new FieldDefinition { Name = "a", TenantId = "tenant2" });

            Assert.False(result);
        }

        /// <summary>
        /// Verifies that <c>FilterByTenant.IsMatch</c> returns <c>true</c> when the <c>TenantId</c> of a <see cref="TestSuite"/> matches the filter.
        /// </summary>
        [Fact]
        public void IsMatch_WithTestSuiteMatch_TrueReturned()
        {
            var specification = new FilterByTenant<TestSuite>("tenant1");
            var result = specification.IsMatch(new TestSuite { Name = "a", TenantId = "tenant1" });

            Assert.True(result);
        }

        /// <summary>
        /// Verifies that <c>FilterByTenant.IsMatch</c> returns <c>false</c> when the <c>TenantId</c> of a <see cref="TestSuite"/> does not match the filter.
        /// </summary>
        [Fact]
        public void IsMatch_TestSuiteNoMatch_FalseReturned()
        {
            var specification = new FilterByTenant<TestSuite>("tenant1");
            var result = specification.IsMatch(new TestSuite { Name = "a", TenantId = "tenant2" });

            Assert.False(result);
        }

        /// <summary>
        /// Verifies that <c>FilterByTenant.IsMatch</c> returns <c>true</c> when the <c>TenantId</c> of a <see cref="TestCase"/> matches the filter.
        /// </summary>
        [Fact]
        public void IsMatch_WithTestCaseMatch_TrueReturned()
        {
            var specification = new FilterByTenant<TestCase>("tenant1");
            var result = specification.IsMatch(new TestCase { Name = "a", TenantId = "tenant1" });

            Assert.True(result);
        }

        /// <summary>
        /// Verifies that <c>FilterByTenant.IsMatch</c> returns <c>false</c> when the <c>TenantId</c> of a <see cref="TestCase"/> does not match the filter.
        /// </summary>
        [Fact]
        public void IsMatch_TestCaseNoMatch_FalseReturned()
        {
            var specification = new FilterByTenant<TestCase>("tenant1");
            var result = specification.IsMatch(new TestCase { Name = "a", TenantId = "tenant2" });

            Assert.False(result);
        }
    }
}