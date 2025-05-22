using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;

using TestBucket.Traits.Core.XmlDoc.Models;

namespace TestBucket.Traits.Core.XmlDoc;
/// <summary>
/// Provides methods for deserializing XML documentation files into <see cref="XmlDocDocument"/> objects.
/// </summary>
public class XmlDocSerializer
{
    /// <summary>
    /// Deserializes an XML documentation file from the specified file path.
    /// </summary>
    /// <param name="filename">The path to the XML documentation file.</param>
    /// <returns>A <see cref="XmlDocDocument"/> representing the deserialized XML documentation.</returns>
    public static XmlDocDocument ParseFile(string filename)
    {
        using var source = File.OpenRead(filename);
        return Deserialize(source);
    }

    /// <summary>
    /// Deserializes an XML documentation file from a string containing XML.
    /// </summary>
    /// <param name="xml">The XML documentation as a string.</param>
    /// <returns>A <see cref="XmlDocDocument"/> representing the deserialized XML documentation.</returns>
    public static XmlDocDocument ParseXml(string xml)
    {
        XmlDocDocument result = new XmlDocDocument();
        XDocument doc = XDocument.Parse(xml);
        Deserialize(result, doc);
        return result;
    }

    /// <summary>
    /// Deserializes an XML documentation file from a file stream.
    /// </summary>
    /// <param name="source">The file stream containing the XML documentation.</param>
    /// <returns>A <see cref="XmlDocDocument"/> representing the deserialized XML documentation.</returns>
    private static XmlDocDocument Deserialize(FileStream source)
    {
        XmlDocDocument result = new XmlDocDocument();
        XDocument doc = XDocument.Load(source);
        Deserialize(result, doc);
        return result;
    }

    /// <summary>
    /// Populates the <see cref="XmlDocDocument"/> with data from the provided <see cref="XDocument"/>.
    /// </summary>
    /// <param name="result">The <see cref="XmlDocDocument"/> to populate.</param>
    /// <param name="doc">The <see cref="XDocument"/> containing the XML documentation.</param>
    private static void Deserialize(XmlDocDocument result, XDocument doc)
    {
        DeserializeAssembly(result, doc);
        DeserializeMembers(result, doc);
    }

    /// <summary>
    /// Deserializes the &lt;member&gt; elements from the XML documentation and adds them to the <see cref="XmlDocDocument.Members"/> collection.
    /// </summary>
    /// <param name="result">The <see cref="XmlDocDocument"/> to populate.</param>
    /// <param name="doc">The <see cref="XDocument"/> containing the XML documentation.</param>
    private static void DeserializeMembers(XmlDocDocument result, XDocument doc)
    {
        var membersElement = doc.Element("doc")?.Element("members");
        if (membersElement == null)
            return;

        foreach (var memberElement in membersElement.Elements("member"))
        {
            var name = memberElement.Attribute("name")?.Value;
            if (string.IsNullOrWhiteSpace(name))
                continue;

            if (TryParseMemberName(name, out var member))
            {
                member.Summary = memberElement.Element("summary")?.Value?.Trim();

                foreach (var paramElem in memberElement.Elements("param"))
                {
                    var paramName = paramElem.Attribute("name")?.Value;
                    var paramDesc = paramElem.Value?.Trim();
                    if (!string.IsNullOrWhiteSpace(paramName))
                    {
                        member.Params.Add(new XmlDocParam(paramName, paramDesc));
                    }
                }

                result.Members.Add(member);
            }
        }
    }

    /// <summary>
    /// Attempts to parse a member name from the XML documentation and create a corresponding <see cref="XmlDocMember"/>.
    /// </summary>
    /// <param name="name">The member name string from the XML documentation (e.g., "M:Namespace.Type.Method").</param>
    /// <param name="member">When this method returns, contains the parsed <see cref="XmlDocMember"/>, if parsing succeeded; otherwise, <c>null</c>.</param>
    /// <returns><c>true</c> if the member name was successfully parsed; otherwise, <c>false</c>.</returns>
    internal static bool TryParseMemberName(string name, [NotNullWhen(true)] out XmlDocMember? member)
    {
        // The type must be 3 characters as it has prefix:name, for example:
        // M:MethodName
        // T:TypeName
        if (name.Length < 3)
        {
            member = null;
            return false;
        }

        var span = name.AsSpan();
        var p = name.IndexOf(':');
        if (p == 1)
        {
            var prefix = span[0];
            var type = XmlDocMemberTypeConverter.GetMemberTypeFromPrefix(prefix);
            member = new XmlDocMember(type, span[2..].ToString());
            return true;
        }

        member = null;
        return false;
    }

    /// <summary>
    /// Deserializes the assembly element from the XML documentation and sets the <see cref="XmlDocDocument.Assembly"/> property.
    /// </summary>
    /// <param name="result">The <see cref="XmlDocDocument"/> to populate.</param>
    /// <param name="doc">The <see cref="XDocument"/> containing the XML documentation.</param>
    private static void DeserializeAssembly(XmlDocDocument result, XDocument doc)
    {
        var assemblyElement = doc.Element("doc")?.Element("assembly");
        if (assemblyElement == null)
            return;

        var assemblyName = assemblyElement.Element("name")?.Value;
        if (assemblyName is not null)
        {
            result.Assembly = new XmlDocAssembly(assemblyName);
        }
    }
}