
using TestBucket.Formats.UnitTests.Utilities;
using TestBucket.Formats.XUnit;
using TestBucket.Traits.Core;
using TestBucket.Traits.XUnit;

namespace TestBucket.Formats.UnitTests.XUnit
{
    [UnitTest]
    [Trait("Format", "XUnit")]
    public class XUnitSerializerTests
    {
        [Fact]
        [TestId("XUNIT-001")]
        [TestDescription("Verifies that a xunit xml run name is from the first assembly")]
        [Component("TestBucket.Formats")]
        public void Deserialize_WithTwoTestSuites_TwoRunsDeserializedWithCorrectNames()
        {
            var xml = TestDataUtils.GetResourceXml("TestBucket.Formats.UnitTests.XUnit.TestData.xunit-properties.xml");
            var serializer = new XUnitSerializer();
            var run = serializer.Deserialize(xml);

            Assert.Equal("TestBucket.Formats.UnitTests.dll", run.Name);
        }

        [Fact]
        [TestId("XUNIT-002")]
        [TestDescription("Verifies that a xunit xml containing traits on the test element are extracted as traits")]
        [Component("TestBucket.Formats")]
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


        [Fact]
        [TestId("XUNIT-003")]
        [TestDescription("Verifies that external ID is set to the TestId if specified")]
        [Component("TestBucket.Formats")]
        public void Deserialize_WithTestIdTrait_ExternalIdSetToTestIdValue()
        {
            var xml = TestDataUtils.GetResourceXml("TestBucket.Formats.UnitTests.XUnit.TestData.xunit-testid.xml");
            var serializer = new XUnitSerializer();
            var run = serializer.Deserialize(xml);

            Assert.NotNull(run.Suites);
            Assert.Single(run.Suites);
            Assert.NotNull(run.Suites[0].Tests);
            var tests = run.Suites[0].Tests;
            Assert.NotNull(tests);
            Assert.Single(tests);

            foreach (var test in tests)
            {
                Assert.NotNull(test.Traits);

                var trait = test.Traits.Where(x => x.Name == TestTraitNames.TestId).FirstOrDefault();
                Assert.NotNull(trait);
                Assert.Equal("MYID-002", trait.Value);
                Assert.Equal("MYID-002", test.ExternalId);
            }
        }
    }
}
