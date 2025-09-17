using Mediator;

using TestBucket.Contracts.Integrations;
using TestBucket.Domain.Issues.Events;
using TestBucket.Domain.Projects;
using TestBucket.Domain.States;

namespace TestBucket.Domain.Issues.Integrations;
internal class CreateExternalIssuesWhenCreated : INotificationHandler<IssueCreated>
{
    private readonly IProjectManager _projectManager;
    private readonly List<IExternalIssueProvider> _externalIssueProviders;
    private readonly IStateService _stateService;
    public CreateExternalIssuesWhenCreated(IProjectManager projectManager, IEnumerable<IExternalIssueProvider> externalIssueProviders, IStateService stateService)
    {
        _projectManager = projectManager;
        _externalIssueProviders = externalIssueProviders.ToList();
        _stateService = stateService;
    }

    public async ValueTask Handle(IssueCreated notification, CancellationToken cancellationToken)
    {
        var issue = notification.Issue;
        var principal = notification.Principal;

        var linker = new LinkIssueWithIntegration(_projectManager, _externalIssueProviders, _stateService);
        await linker.Handle(principal, issue, cancellationToken);
    }
}
