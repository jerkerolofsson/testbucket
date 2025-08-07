using Mediator;

using TestBucket.Domain.Projects;
using TestBucket.Domain.Testing.Events;

namespace TestBucket.Domain.Testing.TestCases.Features;
internal class AssignTeamToTestWhenSavingIfNotAssigned : INotificationHandler<TestCaseSavingEvent>
{
    private readonly IProjectRepository _projectRepo;

    public AssignTeamToTestWhenSavingIfNotAssigned(IProjectRepository projectRepo)
    {
        _projectRepo = projectRepo;
    }

    public async ValueTask Handle(TestCaseSavingEvent notification, CancellationToken cancellationToken)
    {
        var testCase = notification.New;
        var tenantId = notification.Principal.GetTenantIdOrThrow();
        if (testCase.TeamId is null && testCase.TestProjectId is not null)
        {
            var project = await _projectRepo.GetProjectByIdAsync(tenantId, testCase.TestProjectId.Value);
            testCase.TeamId = project?.TeamId;
        }
    }
}
