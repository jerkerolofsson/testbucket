using System.Text;
using TestBucket.Formats.JUnit;
using TestBucket.Formats.UnitTests.Utilities;
using TestBucket.Traits.Xunit;

namespace TestBucket.Formats.UnitTests.JUnit
{
    [UnitTest]
    [Trait("Format", "JUnit")]
    [EnrichedTest]
    public class XunitInteroperabilityTests
    {
        [Fact]
        [TestId("JUNIT-IOT-XUNIT-V3-001")]
        [Component("TestBucket.Formats")]
        [TestDescription("Verifies that a junit xml created by xunit that properties with trait: prefix are removed")]
        public void Deserialize_WithTraitPrefixInProperty_TraitPrefixRemoved()
        {
            var xml = TestDataUtils.GetResourceXml("TestBucket.Formats.UnitTests.JUnit.TestData.created-by-xunit-v3.xml");
            var serializer = new JUnitSerializer();
            var run = serializer.Deserialize(xml);

            Assert.NotEmpty(run.Suites);
            foreach (var suite in run.Suites)
            {
                Assert.NotEmpty(suite.Tests);
                foreach (var test in suite.Tests)
                {
                    foreach(var trait in test.Traits)
                    {
                        Assert.False(trait.Name.StartsWith("trait:"));
                    }
                }
            }
        }

        [Fact]
        [TestId("JUNIT-IOT-XUNIT-V3-002")]
        [TestDescription("Verifies that traits with attachment: prefix are not added as traits")]
        [Component("TestBucket.Formats")]
        public void Deserialize_WithAttachmentPrefixInProperty_AddedAsAttachmentWithCorrectName()
        {
            var xml = TestDataUtils.GetResourceXml("TestBucket.Formats.UnitTests.JUnit.TestData.created-by-xunit-v3-attachment.xml");
            var serializer = new JUnitSerializer();
            var run = serializer.Deserialize(xml);

            Assert.NotEmpty(run.Suites);

            foreach (var suite in run.Suites)
            {
                Assert.NotEmpty(suite.Tests);
                foreach (var test in suite.Tests)
                {
                    foreach (var trait in test.Traits)
                    {
                        Assert.False(trait.Name.StartsWith("attachment:"));
                    }
                    Assert.Empty(test.Attachments);
                    Assert.NotEmpty(test.Traits);
                }
            }
        }

        [Fact]
        [TestId("JUNIT-IOT-XUNIT-V3-003")]
        [TestDescription("Verifies that traits with attachment does not have the attachment: prefix")]
        [Component("TestBucket.Formats")]
        public void Deserialize_WithAttachmentPrefixInPropertyWithoutMimeType_AddedAsAttachmentWithTextPlain()
        {
            var xml = TestDataUtils.GetResourceXml("TestBucket.Formats.UnitTests.JUnit.TestData.created-by-xunit-v3-attachment.xml");
            var serializer = new JUnitSerializer();
            var run = serializer.Deserialize(xml);

            Assert.NotEmpty(run.Suites);

            foreach (var suite in run.Suites)
            {
                Assert.NotEmpty(suite.Tests);
                foreach (var test in suite.Tests)
                {
                    foreach (var trait in test.Traits)
                    {
                        Assert.False(trait.Name.StartsWith("attachment:"));
                    }
                }
            }
        }

        [Fact]
        [TestId("JUNIT-IOT-XUNIT-V3-004")]
        [TestDescription("Verifies that a JUnit XML with attachment and mime-type have correct content type and decoded from base64")]
        [Component("TestBucket.Formats")]
        public void Deserialize_WithAttachmentPrefixInPropertyWithMimeType_AddedAsAttachmentWithCorrectDataAndContentType()
        {
            var xml = TestDataUtils.GetResourceXml("TestBucket.Formats.UnitTests.JUnit.TestData.created-by-xunit-v3-attachment-with-mime.xml");
            var serializer = new JUnitSerializer();
            var run = serializer.Deserialize(xml);

            Assert.NotEmpty(run.Suites);

            foreach (var suite in run.Suites)
            {
                Assert.NotEmpty(suite.Tests);
                foreach (var test in suite.Tests)
                {
                    Assert.NotEmpty(test.Attachments);
                    var attachment = test.Attachments.First();

                    Assert.Equal("application/secret", attachment.ContentType);
                    Assert.NotNull(attachment.Data);
                    Assert.Equal("DESKTOP-E71H2EF", Encoding.UTF8.GetString(attachment.Data));
                }
            }
        }

        [Fact]
        [TestId("JUNIT-IOT-XUNIT-V3-004")]
        [TestDescription("Verifies that name 'Test collection for' and ID is removed from the run name")]
        [Component("TestBucket.Formats")]
        public void Deserialize_WithXunitTestCollectionName_NameTrimmedCorrectly()
        {
            var xml = TestDataUtils.GetResourceXml("TestBucket.Formats.UnitTests.JUnit.TestData.created-by-xunit-v3.xml");
            var serializer = new JUnitSerializer();
            var run = serializer.Deserialize(xml);

            Assert.NotEmpty(run.Suites);
            var suite = run.Suites.First();
            Assert.Equal("TestBucket.Formats.UnitTests.XUnit.XUnitSerializerTests", suite.Name);
        }

        [Fact]
        [TestId("JUNIT-IOT-XUNIT-V3-004")]
        [TestDescription("Verifies that name 'Test collection for' and ID is removed from the run name")]
        [Component("TestBucket.Formats")]
        public void Deserialize_WithXunitImageAttachment_ImageLoadedCorrectly()
        {
            string expectedBase64 = "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";

            var xml = TestDataUtils.GetResourceXml("TestBucket.Formats.UnitTests.JUnit.TestData.created-by-xunit-v3-image-attachment.xml");
            var serializer = new JUnitSerializer();
            var run = serializer.Deserialize(xml);

            Assert.Single(run.Suites);
            var suite = run.Suites.First();
            
            Assert.Single(suite.Tests);
            var test = suite.Tests.First();
            
            Assert.Single(test.Attachments);
            var attachment = test.Attachments.First();

            Assert.Equal("logo.png", attachment.Name);
            Assert.Equal("image/png", attachment.ContentType);
            Assert.Equal(Convert.FromBase64String(expectedBase64), attachment.Data);
        }
    }
}
