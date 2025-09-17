using TestBucket.Contracts.Integrations;
using TestBucket.Domain.ExtensionManagement;
using TestBucket.Domain.Issues.Mapping;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Projects.Mapping;
using TestBucket.Domain.States;

namespace TestBucket.Domain.Issues.Integrations;

/// <summary>
/// Contains logic to link an issue with an external integration.
/// 
/// The integration should be specified as issue.ExternalSystemId
/// 
/// </summary>
public class LinkIssueWithIntegration
{
    private readonly IProjectManager _projectManager;
    private readonly IStateService _stateService;
    private readonly List<IExternalIssueProvider> _externalIssueProviders;

    public LinkIssueWithIntegration(IProjectManager projectManager, IEnumerable<IExternalIssueProvider> externalIssueProviders, IStateService stateService)
    {
        _projectManager = projectManager;
        _externalIssueProviders = externalIssueProviders.ToList();
        _stateService = stateService;
    }

    public async ValueTask Handle(ClaimsPrincipal principal, LocalIssue issue, CancellationToken cancellationToken)
    {
        // Only create an external issue if the system id is set, but the external ID is not set.   
        // If the external id is set, that means we are importing the issue and it already exists in the external system
        if (issue.ExternalSystemId is not null && issue.ExternalId is null)
        {
            // Find integration
            if (issue.TestProjectId is not null)
            {
                var externalSystems = await _projectManager.GetProjectIntegrationsAsync(principal, issue.TestProjectId.Value);
                var externalSystem = externalSystems.FirstOrDefault(x => x.Id == issue.ExternalSystemId);
                if (externalSystem is not null)
                {
                    var provider = _externalIssueProviders.FirstOrDefault(x => x.SystemName == externalSystem.Name);
                    if (provider is not null)
                    {
                        // Update issue in external system
                        var issueDto = issue.ToDto();
                        try
                        {
                            var api = new ExtensionApi(_stateService, principal, issue.TestProjectId.Value);
                            await provider.CreateIssueAsync(api, externalSystem.ToDto(), issueDto, cancellationToken);

                            // If the issue was successfully created, add the ID here..
                            if (!string.IsNullOrEmpty(issueDto.ExternalDisplayId))
                            {
                                issue.ExternalSystemName = externalSystem.Name;
                                issue.ExternalDisplayId = issueDto.ExternalDisplayId;
                                issue.ExternalId = issueDto.ExternalId;
                            }
                            else
                            {
                                // Failed, remove mapping
                                issue.ExternalSystemId = null;
                                issue.ExternalSystemName = null;
                            }
                        }
                        catch
                        {
                            // Failed, remove mapping
                            issue.ExternalSystemId = null;
                            issue.ExternalSystemName = null;

                            throw;
                        }
                    }
                }
            }
        }
    }
}

