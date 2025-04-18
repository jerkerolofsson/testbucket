using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using TestBucket.Domain.Export.Models;
using TestBucket.Domain.Progress;

namespace TestBucket.Domain.Export.Services
{
    /// <summary>
    /// Manages export of a tenant 
    /// </summary>
    internal class BackupManager : IBackupManager
    {
        private readonly Exporter _exporter;
        private readonly IProgressManager _progressManager;
        private readonly ILogger<BackupManager> _logger;
        public BackupManager(Exporter exporter, IProgressManager progressManager, ILogger<BackupManager> logger)
        {
            _exporter = exporter;
            _progressManager = progressManager;
            _logger = logger;
        }

        public async Task CreateBackupAsync(ClaimsPrincipal principal, ExportOptions options)
        {
            var tenantId = principal.GetTenantIdOrThrow();
            _logger.LogInformation("Creating backup for tenant: {TenantId}..", tenantId);

            var date = DateTime.Now;
            var defaultFilename = $"{date.ToString("o")}".Replace(':', '_').Replace('.', '_').Replace('+', '_') + ".zip";
            if (options.DestinationType == ExportDestinationType.Disk)
            {
                AssignDefaultDiskOptions(options, date, defaultFilename);

                await CreateDiskBackupAsync(tenantId, options);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private async Task CreateDiskBackupAsync(string tenantId, ExportOptions options)
        {
            ArgumentNullException.ThrowIfNull(options.Destination);
            await using var progress = _progressManager.CreateProgressTask("Creating backup..");
            using var destinationStream = File.Create(options.Destination);
            await _exporter.ExportFullAsync(options.ExportFormat, tenantId, destinationStream, progress);
        }

        private static void AssignDefaultDiskOptions(ExportOptions options, DateTime date, string defaultFilename)
        {
            if (options.Destination is null)
            {
                var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "test-bucket", "backups", date.Year.ToString(), date.Month.ToString());
                Directory.CreateDirectory(folder);
                options.Destination = Path.Combine(folder, defaultFilename);
            }
        }
    }
}
