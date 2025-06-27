using TestBucket.Domain.Export.Models;

namespace TestBucket.Domain.Export;
public interface IBackupManager
{
    /// <summary>
    /// Creates and saves a backup
    /// </summary>
    /// <param name="principal">User</param>
    /// <param name="options">Backup options</param>
    /// <returns></returns>
    Task CreateBackupAsync(ClaimsPrincipal principal, ExportOptions options);
    Task CreateBackupAsync(ClaimsPrincipal principal, ExportOptions options, Stream destination);
}