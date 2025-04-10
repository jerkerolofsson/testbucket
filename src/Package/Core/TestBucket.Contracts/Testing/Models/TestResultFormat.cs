using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Formats;
public enum TestResultFormat
{
    /// <summary>
    /// JUnit XML
    /// </summary>
    JUnitXml = 0,

    /// <summary>
    /// XUnit XML
    /// </summary>
    xUnitXml = 1,

    /// <summary>
    /// Microsoft TRX XML
    /// </summary>
    MicrosoftTrx = 2,

    /// <summary>
    /// NUnit
    /// </summary>
    NUnitXml = 3,

    /// <summary>
    /// https://ctrf.io/docs/specification/overview
    /// </summary>
    CommonTestReportFormat = 4,

    /// <summary>
    /// Zip file, we will need to recurse
    /// </summary>
    ZipArchive = 5,

    /// <summary>
    /// Format is unknown or could not be detected
    /// </summary>
    UnknownFormat = -1,
}
