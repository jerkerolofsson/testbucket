using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Files.Models;
using TestBucket.Formats;

namespace TestBucket.Domain.Testing.Models;
public class ImportOptions
{
    /// <summary>
    /// Format of file
    /// </summary>
    public TestResultFormat Format { get; set; } = TestResultFormat.JUnitXml;

    /// <summary>
    /// The file to import
    /// </summary>
    public FileResource? File { get; set; }

    /// <summary>
    /// Handling configuration
    /// </summary>
    public ImportHandlingOptions HandlingOptions { get; set; } = new();
}
