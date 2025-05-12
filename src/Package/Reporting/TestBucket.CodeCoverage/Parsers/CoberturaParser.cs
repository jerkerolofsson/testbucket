using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using TestBucket.CodeCoverage.Models;

namespace TestBucket.CodeCoverage.Parsers;
public class CoberturaParser : ParserBase
{
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
            var codeCoverageClass = package.GetOrCreateClass(name.Value, filename?.Value ?? "");

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
            var method = codeCoverageClass.GetOrCreateMethod(name.Value, signature.Value);
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
        return ValueTask.CompletedTask;
    }
}
