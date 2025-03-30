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
    JUnitXml,

    /// <summary>
    /// XUnit XML
    /// </summary>
    xUnitXml,

    /// <summary>
    /// Microsoft TRX XML
    /// </summary>
    MicrosoftTrx,

    /// <summary>
    /// NUnit
    /// </summary>
    NUnitXml,

    /// <summary>
    /// https://ctrf.io/docs/specification/overview
    /// </summary>
    CommonTestReportFormat,
}
