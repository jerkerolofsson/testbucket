using System.Text;

using Mediator;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using TestBucket.Domain.Files.IntegrationEvents;
using TestBucket.Domain.Testing.Services.Import;
using TestBucket.Domain.Testing.TestRuns;
using TestBucket.Formats;

namespace TestBucket.Domain.Automation.Artifact
{
    internal sealed class ImportTestRunFileResourcesWhenAdded : INotificationHandler<FileResourceAddedNotification>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ImportTestRunFileResourcesWhenAdded> _logger;

        public ImportTestRunFileResourcesWhenAdded(IServiceProvider serviceProvider, ILogger<ImportTestRunFileResourcesWhenAdded> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async ValueTask Handle(FileResourceAddedNotification notification, CancellationToken cancellationToken)
        {
            if (notification.Resource.TestRunId is null)
            {
                return;
            }

            var principal = notification.Principal;

            using var scope = _serviceProvider.CreateScope();

            // Verify that we have access to the test run by getting it
            var testRunManager = scope.ServiceProvider.GetRequiredService<ITestRunManager>();
            var testRun = await testRunManager.GetTestRunByIdAsync(principal, notification.Resource.TestRunId.Value);
            if(testRun?.TeamId is null || testRun?.TestProjectId is null)
            {
                return;
            }

            var format = TestResultSerializerFactory.GetFormatFromContentType(notification.Resource.ContentType);
            if(format == TestResultFormat.UnknownFormat)
            {
                _logger.LogDebug("[CI_CD_AUTO] Will not import test run attachment no format was found for media-type: {contentType}", notification.Resource.ContentType);
                return;
            }

            var text = Encoding.UTF8.GetString(notification.Resource.Data);
            var options = new ImportHandlingOptions
            {
                TestRunId = testRun.Id,
                AssignTestsToUserName = principal.Identity?.Name
            };

            _logger.LogDebug("[CI_CD_AUTO] Importing {FileName} using serializer: {format}", notification.Resource.Name, format);
            try
            {
                var textImporter = scope.ServiceProvider.GetRequiredService<ITextTestResultsImporter>();
                await textImporter.ImportTextAsync(principal, testRun.TeamId.Value, testRun.TestProjectId.Value, format, text, options);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to import results: {FileName}", notification.Resource.Name);
            }
        }
    }
}
