using System.Text;
using TestBucket.Formats.Builders;
using TestBucket.Traits.Core;
using TestBucket.Traits.Xunit;

namespace TestBucket.Formats.UnitTests
{
    [Feature("Import Test Results")]
    [Component("TestResultDetector")]
    [UnitTest]
    [EnrichedTest]
    public class TestResultDetectorTests
    {
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
    }
}
