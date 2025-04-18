using System.Security.Claims;

using TestBucket.Domain.Export.Models;

namespace TestBucket.Domain.Export;
public interface IBackupManager
{
    Task CreateBackupAsync(ClaimsPrincipal principal, ExportOptions options);
}