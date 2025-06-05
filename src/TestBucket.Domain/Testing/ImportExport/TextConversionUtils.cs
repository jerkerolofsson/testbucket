using System.Text;

namespace TestBucket.Domain.Testing.ImportExport
{
    /// <summary>
    /// Provides utility methods for converting text from byte arrays, 
    /// handling various encodings and Byte Order Marks (BOM).
    /// </summary>
    public static class TextConversionUtils
    {
        /// <summary>
        /// Converts a byte array to a string, automatically detecting the encoding from the BOM if present.
        /// </summary>
        /// <param name="bytes">The byte array containing the encoded text.</param>
        /// <returns>The decoded string, with the BOM (if any) removed.</returns>
        public static string FromBomEncoded(byte[] bytes)
        {
            Encoding encoding = DetectEncodingFromBom(bytes, out int bomLength);
            return encoding.GetString(bytes, bomLength, bytes.Length - bomLength);
        }

        /// <summary>
        /// Detects the text encoding from the BOM in the provided byte array.
        /// </summary>
        /// <param name="bytes">The byte array to inspect for a BOM.</param>
        /// <param name="bomLength">
        /// Outputs the length of the BOM in bytes, or 0 if no BOM is found.
        /// </param>
        /// <returns>
        /// The detected <see cref="Encoding"/>. Defaults to UTF-8 if no BOM is present.
        /// </returns>
        public static Encoding DetectEncodingFromBom(byte[] bytes, out int bomLength)
        {
            bomLength = 0;
            // UTF-8 BOM: EF BB BF
            if (bytes.Length >= 3 &&
                bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
            {
                bomLength = 3;
                return Encoding.UTF8;
            }
            if (bytes.Length >= 2)
            {
                // UTF-16 BE BOM: FE FF
                if (bytes[0] == 0xFE && bytes[1] == 0xFF)
                {
                    bomLength = 2;
                    return Encoding.BigEndianUnicode; // UTF-16 BE
                }
                // UTF-16 LE BOM: FF FE
                if (bytes[0] == 0xFF && bytes[1] == 0xFE)
                {
                    // UTF-32 LE BOM: FF FE 00 00
                    if (bytes.Length >= 4 && bytes[2] == 0x00 && bytes[3] == 0x00)
                    {
                        bomLength = 4;
                        return Encoding.UTF32; // UTF-32 LE
                    }
                    bomLength = 2;
                    return Encoding.Unicode; // UTF-16 LE
                }
            }
            if (bytes.Length >= 4)
            {
                // UTF-32 BE BOM: 00 00 FE FF
                if (bytes[0] == 0x00 && bytes[1] == 0x00 &&
                    bytes[2] == 0xFE && bytes[3] == 0xFF)
                {
                    bomLength = 4;
                    return new UTF32Encoding(true, true); // UTF-32 BE
                }
            }
            // Default to UTF-8 if no BOM is found
            bomLength = 0;
            return Encoding.UTF8;
        }

        /// <summary>
        /// Converts a UTF-8 encoded byte array to a string, optionally removing the BOM if present.
        /// </summary>
        /// <param name="bytes">The byte array containing UTF-8 encoded text.</param>
        /// <param name="removeBom">
        /// If true, removes the UTF-8 BOM (if present) from the resulting string. Default is true.
        /// </param>
        /// <returns>The decoded string, with the BOM removed if specified.</returns>
        public static string FromUtf8(byte[] bytes, bool removeBom = true)
        {
            if (bytes.Length < 3)
            {
                return Encoding.UTF8.GetString(bytes);
            }

            // Remove the BOM if text contains a BOM
            if (bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
            {
                return Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3);
            }
            return Encoding.UTF8.GetString(bytes);
        }
    }
}