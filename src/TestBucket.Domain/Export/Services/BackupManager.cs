using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using TestBucket.Domain.Export.Models;
using TestBucket.Domain.Progress;
using TestBucket.Domain.Requirements.Models;

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



        public async Task CreateBackupAsync(ClaimsPrincipal principal, ExportOptions options, Stream destination)
        {
            options.DestinationType = ExportDestinationType.Stream;
            options.Destination = null;
            options.DestinationStream = destination;
            await CreateBackupAsync(principal, options);
        }

        /// <summary>
        /// Creates and saves a backup
        /// </summary>
        /// <param name="principal">User</param>
        /// <param name="options">Backup options</param>
        /// <returns></returns>
        public async Task CreateBackupAsync(ClaimsPrincipal principal, ExportOptions options)
        {
            var tenantId = principal.GetTenantIdOrThrow();
            _logger.LogInformation("Creating backup for tenant: {TenantId}..", tenantId);

            var date = DateTime.Now;
            string defaultFilename = GenerateBackupFilenameFromDate(date);
            if (options.DestinationType == ExportDestinationType.Disk)
            {
                AssignDefaultDiskOptions(options, date, defaultFilename);
                await CreateDiskBackupAsync(principal, tenantId, options);
            }
            else if (options.DestinationType == ExportDestinationType.Stream)
            {
                if(options.DestinationStream is null)
                {
                    throw new ArgumentNullException(nameof(options.DestinationStream), "Destination stream cannot be null when using ExportDestinationType.Stream");
                }

                await CreateStreamBackupAsync(principal, tenantId, options, options.DestinationStream);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private static string GenerateBackupFilenameFromDate(DateTime date)
        {
            return $"{date.ToString("o")}".Replace(':', '_').Replace('.', '_').Replace('+', '_') + ".zip";
        }

        /// <summary>
        /// Creates a backup to a file
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="tenantId"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private async Task CreateDiskBackupAsync(ClaimsPrincipal principal, string tenantId, ExportOptions options)
        {
            ArgumentNullException.ThrowIfNull(options.Destination);
            using var destinationStream = File.Create(options.Destination);
            await CreateStreamBackupAsync(principal, tenantId, options, destinationStream);
        }

        private async Task CreateStreamBackupAsync(ClaimsPrincipal principal, string tenantId, ExportOptions options, Stream destinationStream)
        {
            await using var progress = _progressManager.CreateProgressTask("Creating backup..");
            await _exporter.ExportFullAsync(principal, options, tenantId, destinationStream, progress);
        }

        /// <summary>
        /// Adds options needed when saving a backup file to disk
        /// </summary>
        /// <param name="options"></param>
        /// <param name="date"></param>
        /// <param name="defaultFilename"></param>
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
