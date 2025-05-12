using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.CodeCoverage.Models;
public enum CodeCoverageFormat
{
    /// <summary>
    /// Unknown format
    /// </summary>
    UnknownFormat = 0,

    /// <summary>
    /// Cobertura
    /// </summary>
    Cobertura = 1,
}
