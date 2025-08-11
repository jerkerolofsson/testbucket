using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.Export.Events;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Requirements;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Requirements.Specifications.Requirements;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Mapping;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.Specifications.TestCases;

namespace TestBucket.Domain.Export.Handlers.TestSuites;
internal class TestExportHandler : INotificationHandler<ExportNotification>
{
    private readonly ITestCaseRepository _testRepository;
    private readonly IFieldRepository _fieldRepository;
    private readonly IProjectRepository _projectRepository;

    public TestExportHandler(IFieldRepository fieldRepository, ITestCaseRepository testRepository, IProjectRepository projectRepository)
    {
        _fieldRepository = fieldRepository;
        _testRepository = testRepository;
        _projectRepository = projectRepository;
    }


    public async ValueTask Handle(ExportNotification notification, CancellationToken cancellationToken)
    {
        if (!notification.Principal.HasPermission(PermissionEntityType.Requirement, PermissionLevel.Read))
        {
            return;
        }

        string tenantId = notification.TenantId;
        int offset = 0;
        int count = 20;
        var response = await _testRepository.SearchTestSuitesAsync(tenantId, new Testing.TestSuites.Search.SearchTestSuiteQuery {  Count = count, Offset = offset });
        while (response.Items.Length > 0)
        {
            foreach (var item in response.Items)
            {
                if (!notification.Options.Filter(item))
                {
                    continue;
                }
                var dto = item.ToDto();

                // Need to add project reference
                if (item.TestProjectId is not null)
                {
                    item.TestProject = item.TestProject ?? await _projectRepository.GetProjectByIdAsync(tenantId, item.TestProjectId.Value);
                    dto.ProjectSlug = item.TestProject?.Slug;
                }

                await notification.Sink.WriteJsonEntityAsync("tests", "test-suite", item.Id.ToString(), dto, cancellationToken);

                await WriteTestsAsync(notification, tenantId, item, notification.Sink, cancellationToken);
            }

            offset += count;

            response = await _testRepository.SearchTestSuitesAsync(tenantId, new Testing.TestSuites.Search.SearchTestSuiteQuery { Count = count, Offset = offset });
        }
    }

    private async Task WriteTestsAsync(ExportNotification notification, string tenantId, TestSuite testSuite, IDataExporterSink sink, CancellationToken cancellationToken)
    {
        int offset = 0;
        int count = 20;
        var filters = new List<FilterSpecification<TestCase>>
        {
            new FilterByTenant<TestCase>(tenantId),
            new FilterTestCasesByTestSuite(testSuite.Id)
        };
        var response = await _testRepository.SearchTestCasesAsync(offset, count, filters, x => x.Created, false);
        while (response.Items.Length > 0)
        {
            foreach (var item in response.Items)
            {
                if (!notification.Options.Filter(item))
                {
                    continue;
                }
                var dto = item.ToDto();
                dto.TestSuiteSlug = testSuite.Slug;
                dto.ProjectSlug = testSuite.TestProject?.Slug;
                dto.TeamSlug = testSuite.Team?.Slug;

                var fields = await _fieldRepository.GetTestCaseFieldsAsync(tenantId, item.Id);
                dto.Traits = fields.ToTraits();

                await sink.WriteJsonEntityAsync("tests", "test-case", item.Id.ToString(), dto, cancellationToken);
            }

            offset += count;
            response = await _testRepository.SearchTestCasesAsync(offset, count, filters, x => x.Created, false);
        }
    }
}
