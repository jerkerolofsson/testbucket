using TestBucket.Formats.JUnit;
using TestBucket.Formats.UnitTests.Utilities;
using TestBucket.Metrics.Xunit;
using TestBucket.Traits.Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace TestBucket.Formats.UnitTests.JUnit
{
    /// <summary>
    /// JUnitSerializerMemoryTests
    /// </summary>
    [UnitTest]
    [PerformanceTest()]
    [Component("Test Formats")]
    [Feature("Import Test Results")]
    [EnrichedTest]
    [IncludeDiagnostics]
    public class JUnitSerializerMemoryTests
    {
        /// <summary>
        /// Deserializes many times, measuring GC impact
        /// </summary>
        [Fact]
        [TestId("JUNIT-009")]
        public void Deserialize_ManyTimes_ParsedSuccess()
        {
            var xml = TestDataUtils.GetResourceXml("TestBucket.Formats.UnitTests.JUnit.TestData.junit-testid-trait.xml");
            var serializer = new JUnitSerializer();
            var options = new JUnitSerializerOptions();

            Action test = () =>
            {
                serializer.Deserialize(xml, options);
            };

            MemoryTest.Run(test, 50000, 1000);
        }
    }
}
