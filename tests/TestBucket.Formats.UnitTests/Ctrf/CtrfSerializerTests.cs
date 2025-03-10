using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Formats.Ctrf;
using TestBucket.Formats.JUnit;
using TestBucket.Formats.UnitTests.Utilities;
using TestBucket.Traits.Xunit;

namespace TestBucket.Formats.UnitTests.Ctrf
{
    [UnitTest]
    [Trait("Format", "CTRF")]
    [EnrichedTest]
    public class CtrfSerializerTests
    {
        [Fact]
        [TestId("CTRF-001")]
        [TestDescription("Verifies that a deserialized CTRF has a run with StartedTime and EndedTime from the report")]
        [Component("TestBucket.Formats")]
        public void Deserialize_WithStartStopInSummary_StartedAndEndedTimePropertiesSetOnTestRun()
        {
            var json = TestDataUtils.GetResourceXml("TestBucket.Formats.UnitTests.Ctrf.TestData.ctrf-summary.json");
            var serializer = new CtrfXunitSerializer();
            var run = serializer.Deserialize(json);

            Assert.NotNull(run.StartedTime);
            Assert.NotNull(run.EndedTime);
        }

        [Fact]
        [TestId("CTRF-002")]
        [TestDescription("Verifies that a CTRF with a screenshot ")]
        [Component("TestBucket.Formats")]
        public void Deserialize_WithScreenshot_ScreenshotAddedAsAttachment()
        {
            var json = TestDataUtils.GetResourceXml("TestBucket.Formats.UnitTests.Ctrf.TestData.ctrf-screenshot.json");
            var serializer = new CtrfXunitSerializer();
            var run = serializer.Deserialize(json);

            Assert.NotNull(run.Suites);
            Assert.NotEmpty(run.Suites);
            Assert.NotEmpty(run.Suites[0].Tests);
            var test = run.Suites[0].Tests[0];
            Assert.NotEmpty(test.Attachments);

            var attachment = test.Attachments[0];
            Assert.Equal("image/png", attachment.ContentType);

            var expectedBytes = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAPwAAAD8CAMAAABkdTlVAAAAAXNSR0IB2cksfwAAAAlwSFlzAAALEwAACxMBAJqcGAAAAnBQTFRFAAAAfm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//fm//XOyWQwAAANB0Uk5TAGf/0T9AD/08ZdAdVHWVtsva6vPYvaOJbiUWbcX+z3YGQ638ZkqDGq43bPaWDQu05i7fekH3c2s7KWQQ4jXTvwiK2wWF+7lZORkKSWrjf8Axs0wJK/Xo7lwcxMYXzdQMFafJkGmmoGj48gKiDqVQR/lw8JpFEm8jRDSrVfSflHfgiNV+mdd975eksSRO3XggPuywYPo9Wq8H4VcbzJImtwGpTwNfm9ysS3nIGDPCqO3ZLbqhtTLWTWGL677p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");
            Assert.Equal(attachment.Data, expectedBytes);
        }
    }
}
