using System.Text;

using TestBucket.CodeCoverage.Models;

namespace TestBucket.CodeCoverage
{
    /// <summary>
    /// Provides methods to detect the format of code coverage reports.
    /// </summary>
    public static class CoverageFormatDetector
    {
        private const string CoberturaMagic = "<coverage";

        /// <summary>
        /// Detects the code coverage format from a file asynchronously.
        /// </summary>
        /// <param name="filename">The path to the file to analyze.</param>
        /// <returns>
        /// A <see cref="CodeCoverageFormat"/> value indicating the detected format.
        /// </returns>
        public static async Task<CodeCoverageFormat> DetectFromFileAsync(string filename)
        {
            var file = new FileInfo(filename);
            byte[] bytes = new byte[Math.Min(1024, file.Length)];
            using var fileStream = file.OpenRead();
            await fileStream.ReadExactlyAsync(bytes);
            return Detect(bytes);
        }

        /// <summary>
        /// Detects the code coverage format from the provided byte array.
        /// </summary>
        /// <param name="bytes">The byte array containing the file data.</param>
        /// <returns>
        /// A <see cref="CodeCoverageFormat"/> value indicating the detected format.
        /// </returns>
        public static CodeCoverageFormat Detect(byte[] bytes)
        {
            // Assume text
            var text = Encoding.UTF8.GetString(bytes, 0, Math.Min(1500, bytes.Length));
            if (text.Contains(CoberturaMagic))
            {
                return CodeCoverageFormat.Cobertura;
            }

            return CodeCoverageFormat.UnknownFormat;
        }
    }
}