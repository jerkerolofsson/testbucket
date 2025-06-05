using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.Resources;

namespace TestBucket.Domain.UnitTests.NewFolder
{
    /// <summary>
    /// Tests detection of various file formats (media-type) from filename or byte data.
    /// </summary>
    [FunctionalTest]
    [UnitTest]
    [EnrichedTest]
    [Component("File Resources")]
    public class MediaTypeDetectorTests
    {
        /// <summary>
        /// Verifies that the MediaTypeDetector detects the correct content type based on file extension
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="expectedContentType"></param>
        [Theory]
        [InlineData("file.json", "application/json")]
        [InlineData("file.xml", "text/xml")]
        [InlineData("file.txt", "text/plain")]
        [InlineData("file.csv", "text/csv")]
        [InlineData("file.yaml", "application/x-yaml")]
        [InlineData("file.html", "text/html")]
        [InlineData("file.md", "text/markdown")]
        [InlineData("file.pdf", "application/pdf")]
        [InlineData("file.zip", "application/zip")]
        [InlineData("file.png", "image/png")]
        [InlineData("file.jpg", "image/jpeg")]
        [InlineData("file.gif", "image/gif")]
        [InlineData("file.cs", "text/x-csharp")]
        [InlineData("file.razor", "text/x-razor")]
        [InlineData("file.razor.cs", "text/x-csharp")]
        [InlineData("file.unknown", "application/octet-stream")]
        public void DetectType_WithNullContentType_UsesExtensionOrDefault(string fileName, string expectedContentType)
        {
            // Arrange
            byte[] emptyData = Array.Empty<byte>();

            // Act
            var result = MediaTypeDetector.DetectType(fileName, null, emptyData);

            // Assert
            Assert.Equal(expectedContentType, result);
        }


        /// <summary>
        /// Verifies that the correct media type is returned when a content type is provided but filename and contentType is not provided
        /// </summary>
        /// <param name="dataHex"></param>
        /// <param name="expectedContentType"></param>
        [Theory]
        [InlineData("89 50 4e 47", "image/png")]
        [InlineData("47 49 46 38", "image/gif")]
        [InlineData("25 50 44 46 2D", "application/pdf")]
        [InlineData("DF BF 34 EB CE", "application/pdf")]
        [InlineData("50 4B 03 04", "application/zip")]
        [InlineData("37 7A BC AF 27 1C", "application/x-7z-compressed")]
        [InlineData("50 4B 05 06", "application/zip")]
        [InlineData("50 4B 07 08", "application/zip")]
        [InlineData("52 61 72 21 1A 07 00", "application/x-rar-compressed")]
        [InlineData("52 61 72 21 1A 07 01 00", "application/x-rar-compressed")]
        public void DetectType_WithBytes_UsesDataToDetectType(string dataHex, string expectedContentType)
        {
            // Arrange
            string? fileName = null;
            string? contentType = null;
            byte[] data = Convert.FromHexString(dataHex.Replace(" ", ""));

            // Act
            var result = MediaTypeDetector.DetectType(fileName, contentType, data);

            // Assert
            Assert.Equal(expectedContentType, result);
        }

        /// <summary>
        /// Verifies that the correct media type is returned when a content type is provided but filename and contentType is not provided
        /// </summary>
        /// <param name="dataString"></param>
        /// <param name="expectedContentType"></param>
        [Theory]
        [InlineData("{\"key\":\"value\"}", "application/json")] // JSON
        [InlineData("<root></root>", "text/xml")] // XML
        [InlineData("%PDF-1.4", "application/pdf")] // PDF
        [InlineData("<!DOCTYPE html>", "text/html")] // HTML doctype
        [InlineData("<html>", "text/html")] // HTML tag
        public void DetectType_WithTextBytes_UsesDataToDetectType(string dataString, string expectedContentType)
        {
            // Arrange
            string? fileName = null;
            string? contentType = null;
            byte[] data = Encoding.UTF8.GetBytes(dataString);

            // Act
            var result = MediaTypeDetector.DetectType(fileName, contentType, data);

            // Assert
            Assert.Equal(expectedContentType, result);
        }
    }
}
