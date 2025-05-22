using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Traits.Core.XmlDoc.Models;

namespace TestBucket.Traits.Core.XmlDoc;
public class XmlDocMarkdownBuilder
{
    private readonly StringBuilder _stringBuilder = new();

    public XmlDocMarkdownBuilder AddMethod(XmlDocDocument doc, XmlDocMember member)
    {
        _stringBuilder.AppendLine("# " + member.Name);

        if (member.Summary is not null)
        {
            _stringBuilder.AppendLine("## Summary");
            _stringBuilder.AppendLine(member.Summary);
            _stringBuilder.AppendLine();
        }

        var assemblyName = doc.Assembly?.Name ?? "";
        var methodFullName = member.Name;
        var methodName = methodFullName;
        string className = "";
        var methodNameIndex = methodFullName.LastIndexOf('.');
        if(methodNameIndex > 0)
        {
            className = methodFullName[0..methodNameIndex];
            methodName = methodFullName[(methodNameIndex+1)..];
        }

        _stringBuilder.AppendLine("## Source ");
        _stringBuilder.AppendLine();
        _stringBuilder.AppendLine($"| Assembly | Class | Method |");
        _stringBuilder.AppendLine($"| -------- | ----- | ------ |");
        _stringBuilder.AppendLine($"| {assemblyName} | {className} | {methodName} |");
        _stringBuilder.AppendLine();

        if (member.Params.Count > 0)
        {
            _stringBuilder.AppendLine("### Parameters ");
            _stringBuilder.AppendLine();
            _stringBuilder.AppendLine($"| Name     | Summary             |");
            _stringBuilder.AppendLine($"| -------- | ------------------- |");
            foreach (var param in member.Params)
            {
                _stringBuilder.AppendLine($"| {param.Name} | {param.Description} |");
            }
        }
        return this;
    }

    public string Build()
    {
        return _stringBuilder.ToString();
    }
}
