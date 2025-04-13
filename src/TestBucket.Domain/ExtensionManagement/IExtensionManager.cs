
using TestBucket.Contracts.Integrations;

namespace TestBucket.Domain.ExtensionManagement;
public interface IExtensionManager
{
    IExtension? FindExtension(string systemName);
    IReadOnlyList<IExtension> GetExtensions();
    string? GetIcon(string systemName);
}