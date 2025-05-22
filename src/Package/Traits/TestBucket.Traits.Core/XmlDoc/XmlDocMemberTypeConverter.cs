using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Traits.Core.XmlDoc.Models;

namespace TestBucket.Traits.Core.XmlDoc;
public class XmlDocMemberTypeConverter
{
    /// <summary>
    /// Contains a dictionary that converts between the member prefix and an enum
    /// </summary>
    private static FrozenDictionary<char, XmlDocMemberType> _map = new Dictionary<char, XmlDocMemberType>()
    {
        ['M'] = XmlDocMemberType.Method,
        ['T'] = XmlDocMemberType.Type,
        ['F'] = XmlDocMemberType.Field,
        ['P'] = XmlDocMemberType.Property,
    }.ToFrozenDictionary();

    public static XmlDocMemberType GetMemberTypeFromPrefix(char prefix)
    {
        if (_map.TryGetValue(prefix, out XmlDocMemberType type))
        {
            return type;
        }
        return XmlDocMemberType.Unknown;
    }
}
