using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TestBucket.CodeCoverage.Models;

namespace TestBucket.CodeCoverage
{
    public static class CoverageFormatDetector
    {
        private const string CoberturaMagic = "<coverage";

        public static async Task<CodeCoverageFormat> DetectFromFileAsync(string filename)
        {
            var file = new FileInfo(filename);
            byte[] bytes = new byte[Math.Min(1024,file.Length)];
            using var fileStream = file.OpenRead();
            await fileStream.ReadExactlyAsync(bytes);
            return Detect(bytes);
        }

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
