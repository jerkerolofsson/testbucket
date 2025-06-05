using System.Text;
using TestBucket.Domain.Testing.ImportExport;
using Xunit;

namespace TestBucket.Domain.UnitTests.Testing.ImportExport
{
    /// <summary>
    /// TextConversionUtilsTests contains unit tests verifying detection of text encodings from byte arrays with BOMs and 
    /// conversion of BOM-encoded byte arrays to strings.
    /// </summary>
    [EnrichedTest]
    [UnitTest]
    [Component("Testing")]
    public class TextConversionUtilsTests
    {
        /// <summary>
        /// Verifies that DetectEncodingFromBom returns UTF-8 and correct BOM length for a UTF-8 BOM.
        /// </summary>
        [Fact]
        public void DetectEncodingFromBom_WithUtf8Bom_ReturnsUtf8()
        {
            var bytes = new byte[] { 0xEF, 0xBB, 0xBF, 65, 66, 67 };
            var encoding = TextConversionUtils.DetectEncodingFromBom(bytes, out int bomLen);
            Assert.Equal(3, bomLen);
            Assert.Equal(Encoding.UTF8, encoding);
        }

        /// <summary>
        /// Verifies that DetectEncodingFromBom returns UTF-16 LE and correct BOM length for a UTF-16 LE BOM.
        /// </summary>
        [Fact]
        public void DetectEncodingFromBom_WithUtf16LeBom_ReturnsUtf16Le()
        {
            var bytes = new byte[] { 0xFF, 0xFE, 65, 0, 66, 0 };
            var encoding = TextConversionUtils.DetectEncodingFromBom(bytes, out int bomLen);
            Assert.Equal(2, bomLen);
            Assert.Equal(Encoding.Unicode, encoding);
        }

        /// <summary>
        /// Verifies that DetectEncodingFromBom returns UTF-16 BE and correct BOM length for a UTF-16 BE BOM.
        /// </summary>
        [Fact]
        public void DetectEncodingFromBom_WithUtf16BeBom_ReturnsUtf16Be()
        {
            var bytes = new byte[] { 0xFE, 0xFF, 0, 65, 0, 66 };
            var encoding = TextConversionUtils.DetectEncodingFromBom(bytes, out int bomLen);
            Assert.Equal(2, bomLen);
            Assert.Equal(Encoding.BigEndianUnicode, encoding);
        }

        /// <summary>
        /// Verifies that DetectEncodingFromBom returns UTF-32 LE and correct BOM length for a UTF-32 LE BOM.
        /// </summary>
        [Fact]
        public void DetectEncodingFromBom_WithUtf32LeBom_ReturnsUtf32Le()
        {
            var bytes = new byte[] { 0xFF, 0xFE, 0x00, 0x00, 65, 0, 0, 0 };
            var encoding = TextConversionUtils.DetectEncodingFromBom(bytes, out int bomLen);
            Assert.Equal(4, bomLen);
            Assert.Equal(Encoding.UTF32, encoding);
        }

        /// <summary>
        /// Verifies that DetectEncodingFromBom returns UTF-32 BE and correct BOM length for a UTF-32 BE BOM.
        /// </summary>
        [Fact]
        public void DetectEncodingFromBom_WithUtf32BeBom_ReturnsUtf32Be()
        {
            var bytes = new byte[] { 0x00, 0x00, 0xFE, 0xFF, 0, 0, 0, 65 };
            var encoding = TextConversionUtils.DetectEncodingFromBom(bytes, out int bomLen);
            Assert.Equal(4, bomLen);
            Assert.Equal(new UTF32Encoding(true, true), encoding);
        }

        /// <summary>
        /// Verifies that DetectEncodingFromBom defaults to UTF-8 when no BOM is present.
        /// </summary>
        [Fact]
        public void DetectEncodingFromBom_WithNoBom_DefaultsToUtf8()
        {
            var bytes = new byte[] { 1, 2, 3, 4, 5, 6 };
            var encoding = TextConversionUtils.DetectEncodingFromBom(bytes, out int bomLen);
            Assert.Equal(0, bomLen);
            Assert.Equal(Encoding.UTF8, encoding);
        }

        /// <summary>
        /// Verifies that FromBomEncoded decodes a UTF-8 BOM encoded byte array correctly.
        /// </summary>
        [Fact]
        public void FromBomEncoded_WithUtf8Bom_ReturnsDecodedString()
        {
            var text = "Hello";
            var bytes = new byte[] { 0xEF, 0xBB, 0xBF }
                .Concat(Encoding.UTF8.GetBytes(text)).ToArray();
            var result = TextConversionUtils.FromBomEncoded(bytes);
            Assert.Equal(text, result);
        }

        /// <summary>
        /// Verifies that FromBomEncoded decodes a UTF-16 LE BOM encoded byte array correctly.
        /// </summary>
        [Fact]
        public void FromBomEncoded_WithUtf16LeBom_ReturnsDecodedString()
        {
            var text = "Hello";
            var bytes = Encoding.Unicode.GetPreamble()
                .Concat(Encoding.Unicode.GetBytes(text)).ToArray();
            var result = TextConversionUtils.FromBomEncoded(bytes);
            Assert.Equal(text, result);
        }

        /// <summary>
        /// Verifies that FromBomEncoded decodes a UTF-16 BE BOM encoded byte array correctly.
        /// </summary>
        [Fact]
        public void FromBomEncoded_WithUtf16BeBom_ReturnsDecodedString()
        {
            var text = "Hello";
            var bytes = Encoding.BigEndianUnicode.GetPreamble()
                .Concat(Encoding.BigEndianUnicode.GetBytes(text)).ToArray();
            var result = TextConversionUtils.FromBomEncoded(bytes);
            Assert.Equal(text, result);
        }

        /// <summary>
        /// Verifies that FromBomEncoded decodes a UTF-32 LE BOM encoded byte array correctly.
        /// </summary>
        [Fact]
        public void FromBomEncoded_WithUtf32LeBom_ReturnsDecodedString()
        {
            var text = "Hello";
            var bytes = Encoding.UTF32.GetPreamble()
                .Concat(Encoding.UTF32.GetBytes(text)).ToArray();
            var result = TextConversionUtils.FromBomEncoded(bytes);
            Assert.Equal(text, result);
        }

        /// <summary>
        /// Verifies that FromBomEncoded decodes a UTF-32 BE BOM encoded byte array correctly.
        /// </summary>
        [Fact]
        public void FromBomEncoded_WithUtf32BeBom_ReturnsDecodedString()
        {
            var text = "Hello";
            var encoding = new UTF32Encoding(true, true);
            var bytes = encoding.GetPreamble()
                .Concat(encoding.GetBytes(text)).ToArray();
            var result = TextConversionUtils.FromBomEncoded(bytes);
            Assert.Equal(text, result);
        }
    }
}