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
    }
}
