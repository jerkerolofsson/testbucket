using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.Fields.Models;
using TestBucket.Domain.Fields.Specifications;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;
using TestBucket.Traits.TUnit;

namespace TestBucket.Domain.UnitTests.Fields.Specifications
{
    [UnitTest]
    public class FilterByTenantSpecificationTests
    {
        [Test]
        public async Task IsMatch_WithFieldDefinitionsMatch_TrueReturned()
        {
            var specification = new FilterByTenant<FieldDefinition>("tenant1");
            var result = specification.IsMatch(new FieldDefinition { Name = "a", TenantId = "tenant1" });

            await Assert.That(result).IsTrue();
        }
        [Test]
        public async Task IsMatch_FieldDefinitionNoMatch_FalseReturned()
        {
            var specification = new FilterByTenant<FieldDefinition>("tenant1");
            var result = specification.IsMatch(new FieldDefinition { Name = "a", TenantId = "tenant2" });

            await Assert.That(result).IsFalse();
        }

        [Test]
        public async Task IsMatch_WithTestSuiteMatch_TrueReturned()
        {
            var specification = new FilterByTenant<TestSuite>("tenant1");
            var result = specification.IsMatch(new TestSuite { Name = "a", TenantId = "tenant1" });

            await Assert.That(result).IsTrue();
        }
        [Test]
        public async Task IsMatch_TestSuiteNoMatch_FalseReturned()
        {
            var specification = new FilterByTenant<TestSuite>("tenant1");
            var result = specification.IsMatch(new TestSuite { Name = "a", TenantId = "tenant2" });

            await Assert.That(result).IsFalse();
        }

        [Test]
        public async Task IsMatch_WithTestCaseMatch_TrueReturned()
        {
            var specification = new FilterByTenant<TestCase>("tenant1");
            var result = specification.IsMatch(new TestCase { Name = "a", TenantId = "tenant1" });

            await Assert.That(result).IsTrue();
        }
        [Test]
        public async Task IsMatch_TestCaseNoMatch_FalseReturned()
        {
            var specification = new FilterByTenant<TestCase>("tenant1");
            var result = specification.IsMatch(new TestCase { Name = "a", TenantId = "tenant2" });

            await Assert.That(result).IsFalse();
        }
    }
}
