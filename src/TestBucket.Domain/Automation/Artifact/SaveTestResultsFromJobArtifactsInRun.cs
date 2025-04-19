using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using Microsoft.Extensions.DependencyInjection;

using TestBucket.Domain.Automation.Artifact.Events;
using TestBucket.Domain.Files;
using TestBucket.Domain.Testing;
using TestBucket.Domain.Testing.Services.Import;
using TestBucket.Formats;

namespace TestBucket.Domain.Automation.Artifact
{
    /// <summary>
    /// Scans a job zip artifact created in a CI/CD pipeline, and searches for artifacts that matches a defined
    /// pattern for this integration. If matches are found it uploads them as attachments to the test run.
    /// </summary>
    public sealed class SaveTestResultsFromJobArtifactsInRun : INotificationHandler<JobArtifactDownloaded>
    {
        private readonly IServiceProvider _serviceProvider;

        public SaveTestResultsFromJobArtifactsInRun(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async ValueTask Handle(JobArtifactDownloaded notification, CancellationToken cancellationToken)
        {
            if(string.IsNullOrEmpty(notification.TestResultsArtifactsPattern))
            {
                return;
            }
            if (notification.TenantId is null)
            {
                return;
            }

            using var scope = _serviceProvider.CreateScope();
            var files = scope.ServiceProvider.GetRequiredService<IFileResourceManager>();
            var principal = notification.Principal;

            using var stream = new MemoryStream(notification.ZipBytes);
            using var zip = new ZipArchive(stream, ZipArchiveMode.Read);

            var globPatterns = notification.TestResultsArtifactsPattern.Split(';', StringSplitOptions.RemoveEmptyEntries|StringSplitOptions.TrimEntries);
            foreach (var entry in zip.GlobFind(globPatterns))
            {
                // Read into memory
                byte[] bytes = new byte[entry.Length];
                using var entryStream = entry.Open();
                await entryStream.ReadExactlyAsync(bytes, cancellationToken);

                // Detect the file format and add this as an attachment
                var format = TestResultDetector.Detect(bytes);

                await files.AddResourceAsync(principal, new Files.Models.FileResource
                {
                    TenantId = notification.TenantId,
                    Data = bytes,
                    Length = bytes.Length,
                    TestRunId = notification.TestRunId,
                    Name = entry.Name,
                    ContentType = TestResultSerializerFactory.GetContentTypeFromFormat(format) ?? "application/octet-stream"
                });
            }
        }
    }
}
