using TestBucket.Contracts.Integrations;

namespace TestBucket.Domain.ExtensionManagement;

/// <summary>
/// Provides functionality to manage and interact with extensions.
/// </summary>
public interface IExtensionManager
{
    /// <summary>
    /// Finds an extension by its system name.
    /// </summary>
    /// <param name="systemName">The unique system name of the extension.</param>
    /// <returns>The extension if found; otherwise, <c>null</c>.</returns>
    IExtension? FindExtension(string systemName);

    /// <summary>
    /// Retrieves all available extensions.
    /// </summary>
    /// <returns>A read-only list of all extensions.</returns>
    IReadOnlyList<IExtension> GetExtensions();

    /// <summary>
    /// Gets the icon associated with an extension by its system name.
    /// </summary>
    /// <param name="systemName">The unique system name of the extension.</param>
    /// <returns>The icon as a string (e.g., SVG) if available; otherwise, <c>null</c>.</returns>
    string? GetIcon(string systemName);
}