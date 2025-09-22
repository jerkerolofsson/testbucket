using System.Text.RegularExpressions;
using System.Xml.Linq;

using TestBucket.CodeCoverage.CSharp;
using TestBucket.CodeCoverage.Models;

namespace TestBucket.CodeCoverage.Parsers;
public class CoberturaParser : ParserBase
{
    private static readonly Regex LambdaMethodNameRegex = new Regex("<.+>.+__", RegexOptions.Compiled);
    private static readonly Regex CompilerGeneratedMethodNameRegex1 = new Regex(@"(?<ClassName>.+)(/|\.)<\<+(?<CompilerGeneratedName>.+)>.+__.+MoveNext\(\)$", RegexOptions.Compiled);
    private static readonly Regex CompilerGeneratedMethodNameRegex2 = new Regex(@"(?<ClassName>.+)(/|\.)<(?<CompilerGeneratedName>.+)>.+__.+MoveNext\(\)$", RegexOptions.Compiled);
    private static readonly Regex LocalFunctionMethodNameRegex = new Regex(@"^.*(?<ParentMethodName><.+>).*__(?<NestedMethodName>[^\|]+)\|.*$", RegexOptions.Compiled);

    internal static string ExtractMethodName(string methodName, string className)
    {
        if (methodName.Contains("|") || className.Contains("|"))
        {
            Match match = LocalFunctionMethodNameRegex.Match(className + methodName);

            if (match.Success)
            {
                methodName = match.Groups["NestedMethodName"].Value + "()";
            }
        }
        else if (methodName.EndsWith("MoveNext()"))
        {
            Match match = CompilerGeneratedMethodNameRegex1.Match(className + methodName);
            if (match.Success)
            {
                var methodNameFromMatch = match.Groups["CompilerGeneratedName"].Value;
                methodName = methodNameFromMatch + "()";
            }
            else
            {
                Match match2 = CompilerGeneratedMethodNameRegex2.Match(className + methodName);
                if (match2.Success)
                {
                    var methodNameFromMatch = match2.Groups["CompilerGeneratedName"].Value;
                    methodName = methodNameFromMatch + "()";
                }
            }
        }

        methodName = CSharpCleanup.CleanupMethodName(methodName);

        return methodName;
    }


    public override async Task<CodeCoverageReport> ParseStreamAsync(CodeCoverageReport report, Stream stream, CancellationToken cancellationToken)
    {
        XDocument doc = await XDocument.LoadAsync(stream, LoadOptions.None, cancellationToken);

        foreach(var node in doc.Elements("coverage"))
        {
            await ParseCoverageElementAsync(node, report, cancellationToken);
        }

        return report;
    }

    private async ValueTask ParseCoverageElementAsync(XElement coverageNode, CodeCoverageReport report, CancellationToken cancellationToken)
    {
        foreach (var packagesNode in coverageNode.Elements("packages"))
        {
            foreach (var packageNode in packagesNode.Elements("package"))
            {
                cancellationToken.ThrowIfCancellationRequested();
                await ParsePackageElementAsync(packageNode, report, cancellationToken);
            }
        }
    }

    private async ValueTask ParsePackageElementAsync(XElement packageNode, CodeCoverageReport report, CancellationToken cancellationToken)
    {
        var name = packageNode.Attribute("name");
        if(name is not null)
        {
            var package = report.GetOrCreatePackage(name.Value);
            foreach (var classesNode in packageNode.Elements("classes"))
            {
                foreach (var classNode in classesNode.Elements("class"))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await ParseClassElementAsync(classNode, package, report);
                }
            }
        }
    }

    private async ValueTask ParseClassElementAsync(XElement classNode, CodeCoveragePackage package, CodeCoverageReport report)
    {
        var name = classNode.Attribute("name");
        var filename = classNode.Attribute("filename");
        if (name is not null)
        {
            string displayName = CSharp.CSharpCleanup.CleanupClassName(name.Value);

            var codeCoverageClass = package.GetOrCreateClass(displayName, filename?.Value ?? "");

            foreach (var methodsNode in classNode.Elements("methods"))
            {
                foreach (var methodNode in methodsNode.Elements("method"))
                {
                    await ParseMethodNodeAsync(methodNode, codeCoverageClass, report);
                }
            }
            foreach (var linesNode in classNode.Elements("lines"))
            {
                foreach (var lineNode in linesNode.Elements("line"))
                {
                    await ParseLineNodeAsync(lineNode, codeCoverageClass, report);
                }
            }
        }
    }

    private async ValueTask ParseMethodNodeAsync(XElement methodNode, CodeCoverageClass codeCoverageClass, CodeCoverageReport report)
    {
        var name = methodNode.Attribute("name");
        var signature = methodNode.Attribute("signature");
        if (name is not null && signature is not null)
        {
            string fullName = name.Value + signature.Value;
            fullName = ExtractMethodName(fullName, methodNode.Parent!.Parent!.Attribute("name")!.Value);
            if (fullName.Contains("__") && LambdaMethodNameRegex.IsMatch(fullName))
            {
                return;
            }

            // Remove the signature
            if(fullName.Contains('('))
            {
                var p = fullName.IndexOf('(');
                fullName = fullName.Substring(0, p);
            }

            var method = codeCoverageClass.GetOrCreateMethod(fullName, signature.Value);
            foreach (var linesNode in methodNode.Elements("lines"))
            {
                foreach (var lineNode in linesNode.Elements("line"))
                {
                    await ParseLineNodeAsync(lineNode, method, report);
                }
            }
        }
    }

    private ValueTask ParseLineNodeAsync(XElement lineNode, CodeCoverageMethod method, CodeCoverageReport report)
    {
        var lineNumberString = lineNode.Attribute("number");
        if (lineNumberString is not null && int.TryParse(lineNumberString.Value, out var lineNumber))
        {
            var line = method.GetOrCreateLine(lineNumber);
            ParseLineXElement(lineNode, line);
        }
        return ValueTask.CompletedTask;
    }

    private static void ParseLineXElement(XElement lineNode, CodeCoverageLine line)
    {
        line.IsBranch = lineNode.Attribute("branch")?.Value?.ToLower() == "true";
        var hitsAttribute = lineNode.Attribute("hits");
        if (hitsAttribute is not null && int.TryParse(hitsAttribute.Value, out int hits))
        {
            line.Hits += hits;
        }

        foreach (var conditionsNode in lineNode.Elements("conditions"))
        {
            foreach (var conditionNode in conditionsNode.Elements("condition"))
            {
                var conditionNumberAttribute = conditionNode.Attribute("number");
                var conditionTypeAttribute = conditionNode.Attribute("type");
                var conditionCoverageAttribute = conditionNode.Attribute("coverage");
                if (conditionNumberAttribute is not null && int.TryParse(conditionNumberAttribute.Value, out int number))
                {
                    var condition = line.GetOrCreateCondition(number);
                    condition.Type = conditionTypeAttribute?.Value; 
                    if(conditionCoverageAttribute is not null && int.TryParse(conditionCoverageAttribute.Value.TrimEnd('%'), out int coverage))
                    {
                        // There is no good way to merge condition coverage, as it is not clear
                        // if two 50% is the same condition (e.g. a boolean expression) or if it is two different..
                        condition.Coverage = Math.Max(condition.Coverage, coverage/100.0);
                    }
                }
            }
        }
    }

    private ValueTask ParseLineNodeAsync(XElement lineNode, CodeCoverageClass codeCoverageClass, CodeCoverageReport report)
    {
        var lineNumberString = lineNode.Attribute("number");
        if (lineNumberString is not null && int.TryParse(lineNumberString.Value, out var lineNumber))
        {
            // Exclude this line if it is already covered by a method
            foreach(var method in codeCoverageClass.Methods)
            {
                if(method.FindLineByNumber(lineNumber) is not null)
                {
                    return ValueTask.CompletedTask; // Line is already covered by a method
                }
            }

            var line = codeCoverageClass.GetOrCreateLine(lineNumber);
            ParseLineXElement(lineNode, line);
        }
        return ValueTask.CompletedTask;
    }
}
