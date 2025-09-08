using TestBucket.Contracts.Integrations;

namespace TestBucket.Domain.ExtensionManagement
{
    internal class ExtensionManager : IExtensionManager
    {
        private readonly IReadOnlyList<IExtension> _extensions;
        public ExtensionManager(IEnumerable<IExtension> extensions)
        {
            _extensions = extensions.ToList();
        }


        /// <inheritdoc/>
        public IExtension? FindExtension(string systemName)
        {
            return _extensions.FirstOrDefault(e => e.SystemName.Equals(systemName, StringComparison.OrdinalIgnoreCase));
        }

        /// <inheritdoc/>
        public string? GetIcon(string systemName)
        {
            if(systemName == "Test Bucket")
            {
                return TbIcons.Brands.TestBucket;
            }

            var ext = FindExtension(systemName);
            return ext?.Icon;
        }

        /// <inheritdoc/>
        public IReadOnlyList<IExtension> GetExtensions()
        {
            return _extensions;
        }
    }
}
