using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Formats.Ctrf;

namespace TestBucket.Formats
{
    public static class TestResultDetector
    {
        private const string XunitXmlMagic = "<assemblies";
        private const string JUnitXmlMagic = "<testsuites";
        private const string NUnitXmlMagic = "<test-run";
        private const string CtrfMagic = "\"reportFormat\": \"CTRF\"";
        private const string CtrfAltMagic = "\"results\":";
        private const string TrxMagic = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010";

        public static TestResultFormat Detect(byte[] bytes)
        {
            if (bytes.Length > 2 && bytes[0] == 0x0B && bytes[1] == 0x4B) // PK
            {
                return TestResultFormat.ZipArchive;
            }
            else
            {
                // Assume text
                var text = Encoding.UTF8.GetString(bytes, 0, Math.Min(1500, bytes.Length));
                if (text.Contains(XunitXmlMagic))
                {
                    return TestResultFormat.xUnitXml;
                }
                if (text.Contains(TrxMagic))
                {
                    return TestResultFormat.MicrosoftTrx;
                }
                if (text.Contains(JUnitXmlMagic))
                {
                    return TestResultFormat.JUnitXml;
                }
                if (text.Contains(NUnitXmlMagic))
                {
                    return TestResultFormat.NUnitXml;
                }
                if (text.Contains(CtrfMagic))
                {
                    return TestResultFormat.CommonTestReportFormat;
                }
                if (text.Contains(CtrfAltMagic))
                {
                    return TestResultFormat.CommonTestReportFormat;
                }
                if(text.Contains('{'))
                {
                    try
                    {
                        var serializer = new CtrfXunitSerializer();
                        var testRun = serializer.Deserialize(text);

                        // It would be more accurate to check for the mandatory "reportFormat": "CTRF" but it seems some formats don't
                        // output it..
                        if (testRun?.Suites != null)
                        {
                            return TestResultFormat.CommonTestReportFormat;
                        }
                    }
                    catch 
                    { // invalid JSON?
                    }
                }

            }

            return TestResultFormat.UnknownFormat;
        }
    }
}
