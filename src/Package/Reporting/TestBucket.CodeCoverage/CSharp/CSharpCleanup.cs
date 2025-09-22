using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TestBucket.CodeCoverage.CSharp;
internal class CSharpCleanup
{
    private static readonly Regex CleanupRegex = new Regex(".<.*>\\w_?_?\\w*\\d*", RegexOptions.Compiled);
    private static readonly Regex CleanupMethodRegex = new Regex("<(?<MethodName>.*)>\\w_?_?\\w*\\d*", RegexOptions.Compiled);

    private static readonly Regex GenericClassRegex = new Regex("(?<ClassName>.+)(?<GenericTypes><.+>)$", RegexOptions.Compiled);


    public static string CleanupMethodName(string methodName)
    {
        if (methodName.Contains('<'))
        {
            var matchAsync = CleanupMethodRegex.Match(methodName);
            if (matchAsync.Success)
            {
                methodName = matchAsync.Groups["MethodName"].Value;
            }
        }
        return methodName;
    }

    public static string CleanupClassName(string displayName)
    {
        if (displayName.Contains('<'))
        {
            var matchAsync = CleanupRegex.Match(displayName);
            if (matchAsync.Success)
            {
                displayName = CleanupRegex.Replace(displayName, string.Empty);
            }
            var matchGeneric = GenericClassRegex.Match(displayName);
            if (matchGeneric.Success)
            {
                displayName = matchGeneric.Groups["ClassName"].Value;
            }
        }
        return displayName;
    }
}
