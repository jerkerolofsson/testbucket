using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Formats.Shared
{
    /// <summary>
    /// Parses data attachments such as 
    /// data:image/png;base64,aGVsbG93b3JsZA==
    /// data:,Hello%2C%20World%21
    /// data:text/html,lots of text…<p><a name%3D"bottom">bottom</a>?arg=val</p>
    /// 
    /// </summary>
    public class DataUriParser
    {
        public static AttachmentDto ParseDataUri(string uri)
        {
            var attachment = new AttachmentDto();

            if(uri.StartsWith("data:"))
            {
                // data:image/png;base64,aGVsbG93b3JsZA==
                // =>
                // image/png;base64,aGVsbG93b3JsZA==
                uri = uri[5..];
            }

            var p = uri.IndexOf(',');
            if(p > 0)
            { 
                var contentType = uri[0..p];
                var data = uri[(p+1)..];
                var mediaType = contentType;

                // Default to percent-encoded, parse formats such as "image/png;base64"
                string[] contentTypeAttributes = [];
                if (contentType.Contains(';'))
                {
                    var mediaTypeComponents = mediaType.Split(';');
                    mediaType = mediaTypeComponents[0];
                    contentTypeAttributes = mediaTypeComponents[1..].Select(x=>x.ToLower()).ToArray();
                }

                attachment.ContentType = mediaType;
                if (contentTypeAttributes.Contains("base64"))
                {
                    attachment.Data = Convert.FromBase64String(data);
                }
                else
                {
                    // Percent encoded?

                    var isUtf8 = contentTypeAttributes.Contains("utf8") || contentTypeAttributes.Contains("charset=utf-8");

                    data = System.Web.HttpUtility.UrlDecode(data);

                    attachment.Data = Encoding.UTF8.GetBytes(data);
                }
            }

            return attachment;
        }
    }
}
