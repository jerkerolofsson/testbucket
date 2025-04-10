using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.Testing.Services.Import;

namespace TestBucket.Domain.UnitTests.Testing.Services.Import
{
    [EnrichedTest]
    [UnitTest]
    public class ZipGlobExtensionsTests
    {
        [Fact]
        public void GlobFind_WithFilesInRoot_ExtensionWildcard()
        {
            // Arrange
            using var stream = new MemoryStream();
            var zip = GenerateZip(stream, new[] { "test1.txt", "test2.trx" });

            // Act
            var entries = zip.GlobFind(["*.txt"]).ToList();

            // Assert
            Assert.Single(entries);
        }

        [Fact]
        public void GlobFind_WithFilesInSubDir_DirectoryExtensionWildcard_MatchesFound()
        {
            // Arrange
            using var stream = new MemoryStream();
            var zip = GenerateZip(stream, new[] { "a/test1.txt", "b/test2.txt" });

            // Act
            var entries = zip.GlobFind(["**/*.txt"]).ToList();

            // Assert
            Assert.Equal(2, entries.Count);
        }

        [Fact]
        public void GlobFind_WithFilesInSubDir_ExtensionWildcard_MatchesNotFound()
        {
            // Arrange
            using var stream = new MemoryStream();
            var zip = GenerateZip(stream, new[] { "a/test1.txt", "b/test2.trx" });

            // Act
            var entries = zip.GlobFind(["*.txt"]).ToList();

            // Assert
            Assert.Empty(entries);
        }


        [Fact]
        public void GlobFind_WithNoMatches_NoItemsReturned()
        {
            // Arrange
            using var stream = new MemoryStream();
            var zip = GenerateZip(stream, new[] { "test1.txt", "test2.txt" });

            // Act
            var entries = zip.GlobFind(["*.pdf"]).ToList();

            // Assert
            Assert.Empty(entries);
        }

        [Fact]
        public void GlobFind_WithFilesInRoot_ExactMatch()
        {
            // Arrange
            using var stream = new MemoryStream();
            var zip = GenerateZip(stream, new[] { "test1.txt", "test2.txt" });

            // Act
            var entries = zip.GlobFind(["test1.txt"]).ToList();

            // Assert
            Assert.Single(entries);
        }

        [Fact]
        public void GlobFind_WithFilesInRoot_TwoPatterns_ExactMatches()
        {
            // Arrange
            using var stream = new MemoryStream();
            var zip = GenerateZip(stream, new[] { "test1.txt", "test2.txt" });

            // Act
            var entries = zip.GlobFind(["test1.txt", "test2.txt"]).ToList();

            // Assert
            Assert.Equal(2, entries.Count);
        }

        private ZipArchive GenerateZip(Stream stream, string[] paths)
        {
            using (var zip = new ZipArchive(stream, ZipArchiveMode.Create, true))
            {
                foreach(var path in paths)
                {
                    var entry = zip.CreateEntry(path);
                    using var entryStream = entry.Open();
                    entryStream.WriteByte(0);
                }
            }
            stream.Seek(0, SeekOrigin.Begin);
            return new ZipArchive(stream, ZipArchiveMode.Read);
        }
    }
}
