using Microsoft.Extensions.Logging;

using TestBucket.CodeCoverage;
using TestBucket.Domain.Code.CodeCoverage.Models;
using TestBucket.Domain.Files;
using TestBucket.Domain.Files.Models;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Testing.TestRuns;
using TestBucket.Traits.Core;

namespace TestBucket.Domain.Code.CodeCoverage.Import;

public class CodeCoverageImporter
{
    private readonly IFileResourceManager _fileResourceManager;
    private readonly ILogger<CodeCoverageImporter> _logger;
    private readonly ICodeCoverageManager _codeCoverageManager;
    private readonly ITestRunManager _testRunManager;

    public CodeCoverageImporter(IFileResourceManager fileResourceManager, ILogger<CodeCoverageImporter> logger, ICodeCoverageManager codeCoverageManager, ITestRunManager testRunManager)
    {
        _fileResourceManager = fileResourceManager;
        _logger = logger;
        _codeCoverageManager = codeCoverageManager;
        _testRunManager = testRunManager;
    }

    internal async Task ImportAsync(string userName, string tenantId, long resourceId, CancellationToken cancellationToken)
    {
        // Note: User already authenticated before creating job..
        var user = Impersonation.Impersonate(x =>
        {
            x.TenantId = tenantId;
            x.UserName = x.Email = userName;
            x.AddAllPermissions();
        });
        var file = await _fileResourceManager.GetResourceByIdAsync(user, resourceId);
        if(file is null)
        {
            _logger.LogError("Failed to import code coverage. File resource with id {ResourceId} not found.", resourceId);
            return;
        }
        if(!CodeCoverageMediaTypes.IsCodeCoverageFile(file.ContentType))
        {
            _logger.LogError("Failed to import code coverage. File resource with id {ResourceId} is not a valid code coverage file.", resourceId);
            return;
        }

        await SaveCodeCoverageReportAsync(user, file, cancellationToken);
    }

    private async Task SaveCodeCoverageReportAsync(ClaimsPrincipal user, FileResource file, CancellationToken cancellationToken)
    {
        if (file.TestRunId is null)
        {
            _logger.LogError("Failed to import code coverage. TestRun is not defined");
            return;
        }

        var testRun = await _testRunManager.GetTestRunByIdAsync(user, file.TestRunId.Value);
        if (testRun?.TestRunFields is null || testRun?.TestProjectId is null)
        {
            _logger.LogError("Failed to import code coverage. test run not found");
            return;
        }

        var report = await ParseReportAsync(file, cancellationToken);
        if (report is null)
        {
            return;
        }

        // Iterate all groups and update the groups with the new report
        long projectId = testRun.TestProjectId.Value;

        // Save accumulated code coverage for the run
        await SaveCodeCoverageReportAsync(user, file, projectId, CodeCoverageGroupType.TestRun, testRun.Id.ToString(), report, cancellationToken);

        // Save accumulated code coverage for the commit
        var commit = testRun.TestRunFields.FirstOrDefault(x => x.FieldDefinition != null && x.FieldDefinition.TraitType == TraitType.Commit);
        if (!string.IsNullOrEmpty(commit?.StringValue))
        {
            await SaveCodeCoverageReportAsync(user, file, projectId, CodeCoverageGroupType.Commit, commit.StringValue, report, cancellationToken);
        }
    }

    private async Task<CodeCoverageReport?> ParseReportAsync(FileResource file, CancellationToken cancellationToken)
    {
        var parser = CodeCoverageParserFactory.CreateFromContentType(file.ContentType);
        if (parser is null)
        {
            _logger.LogError("Failed to import code coverage. No parser found for content type {ContentType}.", file.ContentType);
            return null;
        }
        var report = new CodeCoverageReport();
        using (var stream = new MemoryStream(file.Data))
        {
            await parser.ParseStreamAsync(report, stream, cancellationToken);
        }
        return report;
    }

    private async Task SaveCodeCoverageReportAsync(ClaimsPrincipal user, FileResource file, long projectId, CodeCoverageGroupType groupType, string groupName, CodeCoverageReport report, CancellationToken cancellationToken)
    {
        CodeCoverageGroup group = await _codeCoverageManager.GetOrCreateCodeCoverageGroupAsync(user, projectId, groupType, groupName);
        group.ResourceIds ??= [];

        CodeCoverageReportMerger reportMerger = new();
        if (!group.ResourceIds.Contains(file.Id))
        {
            group.ResourceIds.Add(file.Id);

            if (group.ResourceIds.Count == 1)
            {
                await UpdateGroupAsync(user, group, report);
            }
            else
            {
                // Merge the reports
                var merged = new CodeCoverageReport();

                // Merge with the newly added report
                merged = reportMerger.Merge(merged, report);

                // Loop over all other resources
                foreach(var id in group.ResourceIds.Where(x=>x != file.Id))
                {
                    var otherFile = await _fileResourceManager.GetResourceByIdAsync(user, id);
                    if(otherFile is null)
                    {
                        _logger.LogWarning("Failed to merge code coverage report. File resource with id {ResourceId} not found.", id);
                        continue;
                    }
                    var oldReport = await ParseReportAsync(otherFile, cancellationToken);
                    if(oldReport is null)
                    {
                        _logger.LogWarning("Failed to merge code coverage report. Failed to parse code coverage report from file resource with id {ResourceId}.", id);
                        continue;
                    }
                    merged = reportMerger.Merge(merged, oldReport);
                }

                await UpdateGroupAsync(user, group, merged);
            }
        }
    }

    private async Task UpdateGroupAsync(ClaimsPrincipal user, CodeCoverageGroup group, CodeCoverageReport report)
    {
        group.LineCount = report.LineCount.Value;
        group.CoveredLineCount = report.CoveredLineCount.Value;
        await _codeCoverageManager.UpdateCodeCoverageGroupAsync(user, group);
    }
}
