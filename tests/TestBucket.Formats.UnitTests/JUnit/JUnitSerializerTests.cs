using TestBucket.Formats.JUnit;
using TestBucket.Formats.UnitTests.Utilities;
using TestBucket.Traits.Xunit;

namespace TestBucket.Formats.UnitTests.JUnit
{
    [Trait("Format", "JUnit")]
    public class JUnitSerializerTests
    {
        /// <summary>
        /// Verifies that a junit xml without a name attribute on the testsuites node gets the name from the first testsuite node
        /// </summary>
        [Fact]
        [TestId("JUNIT-001")]
        [Component("TestBucket.Formats")]
        public void Deserialize_WithoutTestSuitesName_RunNameFromFirstSuite()
        {
            var xml = TestDataUtils.GetResourceXml("TestBucket.Formats.UnitTests.JUnit.TestData.junit-basic.xml");
            var serializer = new JUnitSerializer();
            var run = serializer.Deserialize(xml);

            Assert.NotNull(run.Suites);
            Assert.NotEmpty(run.Suites);
            Assert.Equal(run.Name, run.Suites[0].Name);
        }

        /// <summary>
        /// Verifies that a junit xml is contains the correct number of test suites
        /// </summary>
        [Fact]
        [TestId("JUNIT-002")]
        [Component("TestBucket.Formats")]
        public void Deserialize_WithTwoTestSuites_TwoRunsDeserializedWithCorrectNames()
        {
            var xml = TestDataUtils.GetResourceXml("TestBucket.Formats.UnitTests.JUnit.TestData.junit-basic.xml");
            var serializer = new JUnitSerializer();
            var run = serializer.Deserialize(xml);

            Assert.NotNull(run.Suites);
            Assert.Equal(2, run.Suites.Count);
            Assert.Equal("Tests.Registration", run.Suites[0].Name);
            Assert.Equal("Tests.Authentication", run.Suites[1].Name);
        }

        /// <summary>
        /// Verifies that a junit xml containing properties on the testcase element are extracted as traits
        /// </summary>
        [Fact]
        [TestId("JUNIT-003")]
        [Component("TestBucket.Formats")]
        public void Deserialize_WithPropertiesOnTest_TwoTestsDeserializedWithCorrectTraits()
        {
            var xml = TestDataUtils.GetResourceXml("TestBucket.Formats.UnitTests.JUnit.TestData.junit-properties.xml");
            var serializer = new JUnitSerializer();
            var run = serializer.Deserialize(xml);

            Assert.NotNull(run.Suites);
            Assert.Single(run.Suites);
            Assert.NotNull(run.Suites[0].Tests);
            var tests = run.Suites[0].Tests;
            Assert.NotNull(tests);
            Assert.Equal(2, tests.Count);

            foreach (var test in tests)
            {
                Assert.NotNull(test.Traits);

                var trait = test.Traits.Where(x => x.Name == "Product Component").FirstOrDefault();
                Assert.NotNull(trait);
                Assert.Equal("TestBucket.Formats", trait.Value);
            }
        }
    }
}
