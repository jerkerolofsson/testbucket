
namespace TestBucket.Domain.IntegrationTests.Fixtures
{
    /// <summary>
    /// Represents a temporary directory that is automatically created on instantiation and deleted on disposal.
    /// Useful for managing temporary file system resources in integration tests.
    /// </summary>
    internal class TempDir : IDisposable
    {
        private readonly string _root;

        /// <summary>
        /// Gets the full path of the temporary directory.
        /// </summary>
        public string TempPath => _root;

        /// <summary>
        /// Initializes a new instance of the <see cref="TempDir"/> class.
        /// Creates a unique temporary directory for use during the lifetime of the object.
        /// </summary>
        public TempDir()
        {
            _root = Path.Combine(Path.GetTempPath(), "TestBucket.Domain.IntegrationTests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_root);
        }

        /// <summary>
        /// Deletes the temporary directory and all its contents.
        /// </summary>
        public void Dispose()
        {
            Directory.Delete(_root, true);
        }

        /// <summary>
        /// Returns files in the temp directory matching the pattern
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public FileInfo[] GetFiles(string pattern)
        {
            return new DirectoryInfo(TempPath).GetFiles(pattern);
        }
    }
}