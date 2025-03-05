using System.ComponentModel;
using TestBucket.Formats.UnitTests.Utilities;
using TestBucket.Formats.XUnit;

namespace TestBucket.Formats.UnitTests.XUnit
{
    [Trait("Format", "XUnit")]
    public class XUnitSerializerTests
    {
        /// <summary>
        /// Verifies that a xunit xml run name is from the first assembly
        /// </summary>
        [Fact]
        [Trait("Product Component", "TestBucket.Formats")]
        public void Deserialize_WithTwoTestSuites_TwoRunsDeserializedWithCorrectNames()
        {
            var xml = TestDataUtils.GetResourceXml("TestBucket.Formats.UnitTests.XUnit.TestData.xunit-properties.xml");
            var serializer = new XUnitSerializer();
            var run = serializer.Deserialize(xml);

            Assert.Equal("TestBucket.Formats.UnitTests.dll", run.Name);
        }

        /// <summary>
        /// Verifies that a xunit xml containing traits on the test element are extracted as traits
        /// </summary>
        [Fact]
        [Trait("Description", "Verifies that a xunit xml containing traits on the test element are extracted as traits")]
        [Trait("Product Component", "TestBucket.Formats")]
        public void Deserialize_WithTraitsOnTest_TwoTestsDeserializedWithCorrectTraits()
        {
            var xml = TestDataUtils.GetResourceXml("TestBucket.Formats.UnitTests.XUnit.TestData.xunit-properties.xml");
            var serializer = new XUnitSerializer();
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
