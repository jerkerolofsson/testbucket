using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Integrations;

namespace TestBucket.Domain.ExtensionManagement
{
    class ExtensionManager : IExtensionManager
    {
        private readonly IReadOnlyList<IExtension> _extensions;

        public IExtension? FindExtension(string systemName)
        {
            return _extensions.FirstOrDefault(e => e.SystemName.Equals(systemName, StringComparison.OrdinalIgnoreCase));
        }   

        public string? GetIcon(string systemName)
        {
            var ext = FindExtension(systemName);
            return ext?.Icon;
        }

        public ExtensionManager(IEnumerable<IExtension> extensions)
        {
            _extensions = extensions.ToList();
        }

        public IReadOnlyList<IExtension> GetExtensions()
        {
            return _extensions;
        }
    }
}
