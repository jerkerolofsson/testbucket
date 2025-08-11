using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.Projects;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.TestSuites.Search;

namespace TestBucket.Domain.Testing.Events;
internal class UpdateProjectCountersWhenTestEntityCreatedOrDeleted : 
    INotificationHandler<TestCaseCreatedEvent>,
    INotificationHandler<TestCaseDeletedEvent>,
    INotificationHandler<TestSuiteCreatedEvent>,
    INotificationHandler<TestSuiteDeletedEvent>

{
    private readonly ITestCaseRepository _repository;
    private readonly IProjectRepository _projectRepository;

    public UpdateProjectCountersWhenTestEntityCreatedOrDeleted(ITestCaseRepository repository, IProjectRepository projectRepository)
    {
        _repository = repository;
        _projectRepository = projectRepository;
    }

    public async ValueTask Handle(TestSuiteCreatedEvent notification, CancellationToken cancellationToken)
    {
        if(notification.Suite.TenantId is null ||  notification.Suite.TestProjectId is null)
        {
            return;
        }
        await UpdateTestSuiteCountersAsync(
            notification.Suite.TenantId,
            notification.Suite.TestProjectId);
    }

    public async ValueTask Handle(TestSuiteDeletedEvent notification, CancellationToken cancellationToken)
    {
        if (notification.Suite.TenantId is null || notification.Suite.TestProjectId is null)
        {
            return;
        }
        await UpdateTestSuiteCountersAsync(
            notification.Suite.TenantId,
            notification.Suite.TestProjectId);
    }

    public async ValueTask Handle(TestCaseDeletedEvent notification, CancellationToken cancellationToken)
    {
        if (notification.Test.TenantId is null || notification.Test.TestProjectId is null)
        {
            return;
        }
        await UpdateTestCaseCountersAsync(
            notification.Test.TenantId,
            notification.Test.TestProjectId);
    }

    public async ValueTask Handle(TestCaseCreatedEvent notification, CancellationToken cancellationToken)
    {
        if (notification.Test.TenantId is null || notification.Test.TestProjectId is null)
        {
            return;
        }
        await UpdateTestCaseCountersAsync(
            notification.Test.TenantId,
            notification.Test.TestProjectId);
    }

    private async Task UpdateTestCaseCountersAsync(string tenantId, long? testProjectId)
    {
        if (testProjectId is not null)
        {
            FilterSpecification<TestCase>[] filters = [
                new FilterByProject<TestCase>(testProjectId.Value),
                new FilterByTenant<TestCase>(tenantId)
                ];
            var testCases = await _repository.SearchTestCasesAsync(0, 0, filters, x => x.Created, false);
            var project = await _projectRepository.GetProjectByIdAsync(tenantId, testProjectId.Value);
            if (project is not null)
            {
                project.NumberOfTestCases = (int)testCases.TotalCount;
                await _projectRepository.UpdateProjectAsync(project);
            }
        }
    }
    private async Task UpdateTestSuiteCountersAsync(string tenantId, long? testProjectId)
    {
        if (testProjectId is not null)
        {
            var suites = await _repository.SearchTestSuitesAsync(tenantId, new SearchTestSuiteQuery { ProjectId = testProjectId, Count = 0, Offset = 0 });
            var project = await _projectRepository.GetProjectByIdAsync(tenantId, testProjectId.Value);
            if (project is not null)
            {
                project.NumberOfTestSuites = (int)suites.TotalCount;
                await _projectRepository.UpdateProjectAsync(project);   
            }
        }
    }
}
