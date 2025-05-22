using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.Testing.Duplication;

namespace TestBucket.Domain.UnitTests.Testing.Features.Duplication
{
    /// <summary>
    /// Contains unit tests for <see cref="TestCaseDuplicationExtensions"/> to verify correct duplication of <see cref="TestCase"/> properties.
    /// </summary>
    [UnitTest]
    [EnrichedTest]
    [Feature("Duplication 1.0")]
    public class TestCaseDuplicationExtensionsTests
    {
        /// <summary>
        /// Verifies that the <c>Name</c> property is copied and suffixed with " copy" when duplicating a test case.
        /// </summary>
        [Fact]
        public void DuplicateTest_CopiesName()
        {
            var test = new TestCase { Name = "name1", Description = "description1" };

            var cloned = test.Duplicate();

            Assert.Equal(test.Name + " copy", cloned.Name);
        }

        /// <summary>
        /// Verifies that the <c>Description</c> property is copied when duplicating a test case.
        /// </summary>
        [Fact]
        public void DuplicateTest_CopiesDescription()
        {
            var test = new TestCase { Name = "name1", Description = "description1" };

            var cloned = test.Duplicate();

            Assert.Equal(test.Description, cloned.Description);
        }

        /// <summary>
        /// Verifies that the <c>RunnerLanguage</c> property is copied when duplicating a test case.
        /// </summary>
        [Fact]
        public void DuplicateTest_CopiesRunnerLanguage()
        {
            var test = new TestCase { Name = "name1", Description = "description1", RunnerLanguage = "javascript" };

            var cloned = test.Duplicate();

            Assert.Equal(test.RunnerLanguage, cloned.RunnerLanguage);
        }

        /// <summary>
        /// Verifies that the <c>ExecutionType</c> property is copied when duplicating a test case.
        /// </summary>
        [Fact]
        public void DuplicateTest_CopiesExecutionType()
        {
            var test = new TestCase { Name = "name1", Description = "description1", ExecutionType = Contracts.Testing.Models.TestExecutionType.Hybrid };

            var cloned = test.Duplicate();

            Assert.Equal(test.ExecutionType, cloned.ExecutionType);
        }
    }
}