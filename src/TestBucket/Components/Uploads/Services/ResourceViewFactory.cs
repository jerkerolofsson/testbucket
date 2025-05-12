using TestBucket.Components.Uploads.Views;
using TestBucket.Domain.Files.Models;

namespace TestBucket.Components.Uploads.Services;

public class ResourceViewFactory
{
    public IEnumerable<ViewType> GetViewTypesForPreview(FileResource resource)
    {
        if(resource.ContentType != null)
        {
            if (resource.ContentType.StartsWith("image/"))
            {
                yield return new ViewType("Image", typeof(ImageViewer));
            }
            if (resource.ContentType.StartsWith("application/json"))
            {
                yield return new ViewType("JSON", typeof(JsonViewer));
            }
            if (resource.ContentType.StartsWith("application/x-cobertura"))
            {
                yield return new ViewType("Code Coverage", typeof(CodeCoverageResourceView));
            }
            if (resource.ContentType.StartsWith("application/xml") ||
                resource.ContentType.StartsWith("application/x-xunit") ||
                resource.ContentType.StartsWith("application/x-junit") ||
                resource.ContentType.StartsWith("text/xml"))
            {
                yield return new ViewType("XML", typeof(XmlViewer));
            }
            if (resource.ContentType.StartsWith("text/") ||
                resource.ContentType.StartsWith("aplication/xml") ||
                resource.ContentType.StartsWith("aplication/json"))
            {
                yield return new ViewType("Plain Text", typeof(PlainTextView));
            }
        }

        yield return new ViewType("Hex", typeof(HexView));
    }
}

