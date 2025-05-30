﻿using System.IO.Compression;

using Mediator;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using TestBucket.Domain.Automation.Artifact.Events;
using TestBucket.Domain.Files;
using TestBucket.Domain.Resources;
using TestBucket.Domain.Testing.Services.Import;

namespace TestBucket.Domain.Automation.Artifact
{
    /// <summary>
    /// Scans a job zip artifact created in a CI/CD pipeline, and searches for artifacts that matches a defined
    /// pattern for this integration. If matches are found it uploads them as attachments to the test run.
    /// </summary>
    public sealed class SaveTestResultsFromJobArtifactsInRun : INotificationHandler<JobArtifactDownloaded>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SaveTestResultsFromJobArtifactsInRun> _logger;

        public SaveTestResultsFromJobArtifactsInRun(IServiceProvider serviceProvider, ILogger<SaveTestResultsFromJobArtifactsInRun> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async ValueTask Handle(JobArtifactDownloaded notification, CancellationToken cancellationToken)
        {
            if (notification.ZipBytes.Length == 0)
            {
                // Fast exit for empty zips
                _logger.LogWarning("[CI_CD_AUTO] zip is zero bytes");
                return;
            }

            if (string.IsNullOrEmpty(notification.TestResultsArtifactsPattern))
            {
                // If no pattern was defined, we cannot do anything
                _logger.LogDebug("[CI_CD_AUTO] glob-pattern for test results is not set in JobArtifactDownloaded notification");
                return;
            }
            try
            {
                await ScanZipAsync(notification, cancellationToken);
            }
            catch(Exception ex)
            {
                // Corrupt files etc. just log it
                _logger.LogError(ex, "Failed to scan artifact zip");
            }
        }

        private async Task ScanZipAsync(JobArtifactDownloaded notification, CancellationToken cancellationToken)
        {
            var tenantId = notification.Principal.GetTenantIdOrThrow();
            using var scope = _serviceProvider.CreateScope();
            var files = scope.ServiceProvider.GetRequiredService<IFileResourceManager>();
            var principal = notification.Principal;

            using var stream = new MemoryStream(notification.ZipBytes);
            using var zip = new ZipArchive(stream, ZipArchiveMode.Read);

            var globPatterns = notification.TestResultsArtifactsPattern.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var entry in zip.GlobFind(globPatterns))
            {
                // Read into memory
                byte[] bytes = new byte[entry.Length];
                using var entryStream = entry.Open();
                await entryStream.ReadExactlyAsync(bytes, cancellationToken);

                // Detect the file format and add this as an attachment
                var contentType = MediaTypeDetector.DetectType("application/octet-stream", bytes);

                _logger.LogDebug("[CI_CD_AUTO] Processing artifact zip-entry: {ArtifactZipEntryName}, length={ArtifactZipEntryLength} bytes, type={ContentType}", entry.Name, entry.Length, contentType);

                await files.AddResourceAsync(principal, new Files.Models.FileResource
                {
                    TenantId = principal.GetTenantIdOrThrow(),
                    Data = bytes,
                    Length = bytes.Length,
                    TestRunId = notification.TestRunId,
                    Name = entry.Name,
                    ContentType = contentType
                });
            }
        }
    }
}
