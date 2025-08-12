using System.Xml.Linq;

namespace TestBucket.AdbProxy.Appium.PageSources;
public class XmlPageSource
{
    public int Rotation { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public List<HierarchyNode> Nodes { get; set; } = new();

    public static XmlPageSource FromString(string xml)
    {
        XmlPageSource source = new XmlPageSource();

        XDocument doc = XDocument.Parse(xml);

        XElement? root = doc.Root;
        if (root is not null)
        {
            source.Rotation = int.Parse(root.Attribute("rotation")?.Value ?? "0");
            source.Width = int.Parse(root.Attribute("width")?.Value ?? "0");
            source.Height = int.Parse(root.Attribute("height")?.Value ?? "0");

            foreach (var node in root.Elements())
            {
                ParseElement(null, node, source.Nodes);
            }
        }

        return source;
    }

    private static void ParseElement(HierarchyNode? parent, XElement element, List<HierarchyNode> nodes)
    {
        HierarchyNode node = new HierarchyNode
        {
            Parent = parent,
            TagName = element.Name.LocalName,
            Index = int.Parse(element.Attribute("index")?.Value ?? "0"),
            ClassName = element.Attribute("class")?.Value,
            PackageName = element.Attribute("package")?.Value,
            ResourceId = element.Attribute("resource-id")?.Value,
            Text = element.Attribute("text")?.Value,
            ContentDescription = element.Attribute("content-desc")?.Value,
            Checked = ParseNullableBool(element.Attribute("checked")?.Value),
            Checkable = ParseNullableBool(element.Attribute("checkable")?.Value),
            Scrollable = ParseNullableBool(element.Attribute("scrollable")?.Value),
            Displayed = ParseNullableBool(element.Attribute("displayed")?.Value),
            LongClickable = ParseNullableBool(element.Attribute("long-clickable")?.Value),
            Clickable = ParseNullableBool(element.Attribute("clickable")?.Value),
            Enabled = ParseNullableBool(element.Attribute("enabled")?.Value),
            Focusable = ParseNullableBool(element.Attribute("focusable")?.Value),
            Password = ParseNullableBool(element.Attribute("password")?.Value),
            A11yImportant = ParseNullableBool(element.Attribute("a11y-important")?.Value),
            ScreenReaderFocusable = ParseNullableBool(element.Attribute("screen-reader-focusable")?.Value),
            ShowingHint = ParseNullableBool(element.Attribute("showing-hint")?.Value),
            ContextClickable = ParseNullableBool(element.Attribute("context-clickable")?.Value),
            ContextInvalid = ParseNullableBool(element.Attribute("content-invalid")?.Value),
            Heading = ParseNullableBool(element.Attribute("heading")?.Value),
            DrawingOrder = ParseNullableInt(element.Attribute("drawing-order")?.Value),
            LiveRegion = ParseNullableInt(element.Attribute("live-region")?.Value),
            X = ParseBoundary(element.Attribute("bounds")?.Value, "x"),
            Y = ParseBoundary(element.Attribute("bounds")?.Value, "y"),
            Width = ParseBoundary(element.Attribute("bounds")?.Value, "width"),
            Height = ParseBoundary(element.Attribute("bounds")?.Value, "height")
        };

        nodes.Add(node);

        foreach (var childNode in element.Elements())
        {
            ParseElement(node, childNode, node.Nodes);
        }
    }

    private static bool? ParseNullableBool(string? value)
    {
        return value switch
        {
            "true" => true,
            "false" => false,
            _ => null
        };
    }

    private static int? ParseNullableInt(string? value)
    {
        return int.TryParse(value, out int result) ? result : null;
    }

    private static int ParseBoundary(string? bounds, string type)
    {
        if (string.IsNullOrEmpty(bounds)) return 0;

        // [0,0][1096,2491]

        bounds = bounds.Replace("][", ",");
        var parts = bounds.Trim('[', ']').Split(',');
        if (parts.Length != 4)
        {
            return 0;
        }

        if (type == "x") return int.TryParse(parts[0], out var x) ? x : 0;
        if (type == "y") return int.TryParse(parts[1], out var y) ? y : 0;

        if (type == "width") return int.TryParse(parts[2], out var w) ? w : 0;
        if (type == "height") return int.TryParse(parts[3], out var h) ? h : 0;

        return 0;
    }

}
