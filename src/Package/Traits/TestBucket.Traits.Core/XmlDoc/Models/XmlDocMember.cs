using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Traits.Core.XmlDoc.Models;
public record class XmlDocMember(XmlDocMemberType Type, string Name)
{
    public string? Summary { get; set; }

    public List<XmlDocParam> Params { get; set; } = [];
}
