using System.Xml.Linq;

namespace Brightest.Testing.Domain.Formats.Xml
{
    public static class XDocumentExtensions
    {
        public static void CleanXmlNamespaces(this XDocument doc)
        {
            if (doc.Root is null)
            {
                return;
            }
            foreach (var node in doc.Root.Descendants())
            {
                if (node.Name.NamespaceName == "")
                {
                    node.Attributes("xmlns").Remove();
                    if (node.Parent is not null)
                    {
                        node.Name = node.Parent.Name.Namespace + node.Name.LocalName;
                    }
                }
            }
        }
    }
}
