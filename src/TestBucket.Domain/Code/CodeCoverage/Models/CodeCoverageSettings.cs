using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Code.CodeCoverage.Models;
public class CodeCoverageSettings
{
    /// <summary>
    /// Stretch target in percentage, default is 90%
    /// </summary>
    public int StretchTarget { get; set; } = 90;

    /// <summary>
    /// Target in percentage, default is 80%
    /// </summary>
    public int Target { get; set; } = 80;

    /// <summary>
    /// Min target in percentage, default is 70%
    /// </summary>
    public int MinTarget { get; set; } = 70;

    /// <summary>
    /// Automatically imports code coverage report attachments when added to test runs
    /// </summary>
    public bool AutomaticallyImportTestRunCodeCoverageReports { get; set; }
}
