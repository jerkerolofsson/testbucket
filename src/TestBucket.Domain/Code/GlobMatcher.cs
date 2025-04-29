using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DotNet.Globbing;

namespace TestBucket.Domain.Code;
internal class GLobMatcher
{
    public static bool IsMatch(string filePath, IEnumerable<string> globPatterns)
    {
        foreach (var pattern in globPatterns)
        {
            if (pattern.StartsWith('!'))
            {
                if (Glob.Parse(pattern.Substring(1)).IsMatch(filePath))
                {
                    return false;
                }
            }

            if (Glob.Parse(pattern).IsMatch(filePath))
            {
                return true;
            }
        }
        return false;
    }
}
