using System.Text;
using TestBucket.Formats.Builders;
using TestBucket.Formats.UnitTests.Utilities;
using TestBucket.Traits.Core;
using TestBucket.Traits.Xunit;

namespace TestBucket.Formats.UnitTests
{
    [FunctionalTest]
    [Component("Test Result Formats")]
    [Feature("Import Test Results")]
    [UnitTest]
    [EnrichedTest]
    public class TestResultDetectorTests
    {
        [Fact]
        public async Task DetectFromFileAsync_WithZip_ResultIsZipArchive()
        {
            var format = await TestResultDetector.DetectFromFileAsync("./Zip/TestData/junit-no-testsuites.zip");
            Assert.Equal(TestResultFormat.ZipArchive, format);
        }

        [Fact]
        public void Detect_WithTrx_ResultIsTrx()
        {
            var text = """
                <?xml version="1.0" encoding="utf-8"?>
                <TestRun id="d5b3083c-dff7-436b-8a75-4c24d135d53e" name="amauryleve@MYMACHINE 2022-12-01 13:25:00" runUser="EUROPE\amauryleve" xmlns="http://microsoft.com/schemas/VisualStudio/TeamTest/2010">
                </TestRun>
                """;
            var format = TestResultDetector.Detect(Encoding.UTF8.GetBytes(text));
            Assert.Equal(TestResultFormat.MicrosoftTrx, format);
        }


        [Fact]
        public void Detect_WithCtrfJsoncheck_ResultIsCtrf()
        {
            var json = TestDataUtils.GetResourceXml("TestBucket.Formats.UnitTests.Ctrf.TestData.ctrf-summary.json");
            var sb = new StringBuilder();
            for(int i=0; i<2000; i++)
            {
                sb.Append("            ");
            }

            var format = TestResultDetector.Detect(Encoding.UTF8.GetBytes(sb.ToString() + json));
            Assert.Equal(TestResultFormat.CommonTestReportFormat, format);
        }

        [Fact]
        public void Detect_WithCtrfMagic_ResultIsCtrf()
        {
            var text = """
                {
                "reportFormat": "CTRF"
                }
                """;
            var format = TestResultDetector.Detect(Encoding.UTF8.GetBytes(text));
            Assert.Equal(TestResultFormat.CommonTestReportFormat, format);
        }


        [Fact]
        public void Detect_WithCtrfMagicAlt_ResultIsCtrf()
        {
            var text = """
                {
                "results": {
                  "environment": {
                    "osPlatform": "Windows",
                    "osRelease": "Microsoft Windows 10.0.19045"
                  }
                }
                """;
            var format = TestResultDetector.Detect(Encoding.UTF8.GetBytes(text));
            Assert.Equal(TestResultFormat.CommonTestReportFormat, format);
        }

        [Fact]
        public void Detect_WithReportFormatCtrf_ResultIsCtrf()
        {
            var text = "{\"reportFormat\": \"CTRF\"}";
            var format = TestResultDetector.Detect(Encoding.UTF8.GetBytes(text));
            Assert.Equal(TestResultFormat.CommonTestReportFormat, format);
        }

        [Fact]
        public void Detect_WithJUnitXml_ResultIsJUnit()
        {
            var text = """
                <?xml version="1.0" encoding="utf-8"?>
                <testsuites name="Test results" time="0.103" tests="9" failures="0" errors="0" skipped="0">
                </testsuites>
                """;
                
            var format = TestResultDetector.Detect(Encoding.UTF8.GetBytes(text));
            Assert.Equal(TestResultFormat.JUnitXml, format);
        }

        [Fact]
        public void Detect_WithXUnitXml_ResultIsXUnit()
        {
            var text = """
                <?xml version="1.0" encoding="utf-8"?>
                <assemblies>
                </assemblies>
                """;

            var format = TestResultDetector.Detect(Encoding.UTF8.GetBytes(text));
            Assert.Equal(TestResultFormat.xUnitXml, format);
        }

        [Fact]
        public void Detect_WithEmptyInput_ResultIsUnknown()
        {
            var format = TestResultDetector.Detect(Array.Empty<byte>());
            Assert.Equal(TestResultFormat.UnknownFormat, format);
        }

        [Fact]
        public void Detect_WithRandomData_ResultIsUnknown()
        {
            var randomBytes = Encoding.UTF8.GetBytes("this is not a test result file");
            var format = TestResultDetector.Detect(randomBytes);
            Assert.Equal(TestResultFormat.UnknownFormat, format);
        }

        [Fact]
        public void Detect_WithMalformedXml_ResultIsUnknown()
        {
            var text = "<notclosed";
            var format = TestResultDetector.Detect(Encoding.UTF8.GetBytes(text));
            Assert.Equal(TestResultFormat.UnknownFormat, format);
        }

        [Fact]
        public void Detect_WithMalformedJson_ResultIsUnknown()
        {
            var text = "{ not: valid json";
            var format = TestResultDetector.Detect(Encoding.UTF8.GetBytes(text));
            Assert.Equal(TestResultFormat.UnknownFormat, format);
        }

    }
}
