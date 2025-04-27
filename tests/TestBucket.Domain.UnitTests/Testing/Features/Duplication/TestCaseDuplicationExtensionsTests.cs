using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.Testing.Duplication;

namespace TestBucket.Domain.UnitTests.Testing.Features.Duplication
{
    [UnitTest]
    [EnrichedTest]
    [Feature("Duplication 1.0")]
    public class TestCaseDuplicationExtensionsTests
    {
        [Fact]
        [TestDescription("Verifies that the name is copied when duplicating a test")]
        public void DuplicateTest_CopiesName()
        {
            var test = new TestCase { Name = "name1", Description = "description1" };

            var cloned = test.Duplicate();

            Assert.Equal(test.Name + " copy", cloned.Name);
        }

        [Fact]
        [TestDescription("Verifies that the description is copied when duplicating a test")]
        public void DuplicateTest_CopiesDescription()
        {
            var test = new TestCase { Name = "name1", Description = "description1" };

            var cloned = test.Duplicate();

            Assert.Equal(test.Description, cloned.Description);
        }

        [Fact]
        [TestDescription("Verifies that the RunnerLanguage is copied when duplicating a test")]
        public void DuplicateTest_CopiesRunnerLanguage()
        {
            var test = new TestCase { Name = "name1", Description = "description1", RunnerLanguage = "javascript" };

            var cloned = test.Duplicate();

            Assert.Equal(test.RunnerLanguage, cloned.RunnerLanguage);
        }
        [Fact]
        [TestDescription("Verifies that the ExecutionType is copied when duplicating a test")]
        public void DuplicateTest_CopiesExecutionType()
        {
            var test = new TestCase { Name = "name1", Description = "description1", ExecutionType = Contracts.Testing.Models.TestExecutionType.Hybrid };

            var cloned = test.Duplicate();

            Assert.Equal(test.ExecutionType, cloned.ExecutionType);
        }

    }
}
