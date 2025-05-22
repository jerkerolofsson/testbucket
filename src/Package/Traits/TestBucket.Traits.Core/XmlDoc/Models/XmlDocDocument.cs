using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Traits.Core.XmlDoc.Models;

/// <summary>
/// Contains a deserialized xml document for an assembly
/// </summary>
public class XmlDocDocument
{
    public XmlDocAssembly? Assembly { get; set; }

    /// <summary>
    /// List of all members
    /// </summary>
    public List<XmlDocMember> Members { get; set; } = [];

    public IEnumerable<XmlDocMember> Types => Members.Where(x => x.Type == XmlDocMemberType.Type);
    public IEnumerable<XmlDocMember> Methods => Members.Where(x => x.Type == XmlDocMemberType.Method);
}
