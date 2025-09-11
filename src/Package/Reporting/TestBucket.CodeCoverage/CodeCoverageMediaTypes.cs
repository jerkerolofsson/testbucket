using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.CodeCoverage;

public class CodeCoverageMediaTypes
{
    public const string Cobertura = "application/x-cobertura";

    public static bool IsCodeCoverageFile(string? mediaType)
    {
        if(string.IsNullOrWhiteSpace(mediaType))
        {
            return false;
        }

        if(mediaType.StartsWith(Cobertura, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return false;
    }
}
