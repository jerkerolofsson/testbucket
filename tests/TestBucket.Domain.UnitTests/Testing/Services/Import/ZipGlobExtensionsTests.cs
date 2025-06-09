using System.IO.Compression;
using TestBucket.Domain.Testing.Services.Import;

namespace TestBucket.Domain.UnitTests.Testing.Services.Import
{
    /// <summary>
    /// Contains unit tests for <see cref="ZipGlobExtensions"/> and its <c>GlobFind</c> extension method for matching files in a <see cref="ZipArchive"/> using glob patterns.
    /// </summary>
    [EnrichedTest]
    [UnitTest]
    [Component("Testing")]
    [FunctionalTest]
    public class ZipGlobExtensionsTests
    {
        /// <summary>
        /// Verifies that <c>GlobFind</c> matches files in the root of the zip archive using an extension wildcard pattern.
        /// </summary>
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

        /// <summary>
        /// Verifies that <c>GlobFind</c> matches files in subdirectories using a directory extension wildcard pattern.
        /// </summary>
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

        /// <summary>
        /// Verifies that <c>GlobFind</c> does not match files in subdirectories when using a root extension wildcard pattern.
        /// </summary>
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

        /// <summary>
        /// Verifies that <c>GlobFind</c> returns no items when there are no matches for the given pattern.
        /// </summary>
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

        /// <summary>
        /// Verifies that <c>GlobFind</c> matches a file in the root of the zip archive using an exact file name.
        /// </summary>
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

        /// <summary>
        /// Verifies that <c>GlobFind</c> matches multiple files in the root using multiple exact file name patterns.
        /// </summary>
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

        /// <summary>
        /// Generates a <see cref="ZipArchive"/> in memory with the specified file paths.
        /// </summary>
        /// <param name="stream">The stream to write the zip archive to.</param>
        /// <param name="paths">The file paths to include in the zip archive.</param>
        /// <returns>A <see cref="ZipArchive"/> containing the specified files.</returns>
        private ZipArchive GenerateZip(Stream stream, string[] paths)
        {
            using (var zip = new ZipArchive(stream, ZipArchiveMode.Create, true))
            {
                foreach (var path in paths)
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