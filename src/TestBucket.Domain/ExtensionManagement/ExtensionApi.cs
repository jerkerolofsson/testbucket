using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Issues.States;
using TestBucket.Domain.States;
using TestBucket.Domain.States.Models;
using TestBucket.Integrations;

namespace TestBucket.Domain.ExtensionManagement;

internal class ExtensionApi : IExtensionApi
{
    private readonly IStateService _stateService;
    private readonly ClaimsPrincipal _principal;
    private readonly long _projectId;

    public ExtensionApi(IStateService stateService, ClaimsPrincipal principal, long projectId)
    {
        _stateService = stateService;
        _principal = principal;
        _projectId = projectId;
    }

    public async Task<IStateTranslator> GetStateTranslatorAsync()
    {
        var issueStates = await _stateService.GetIssueStatesAsync(_principal, _projectId);
        return new StateTranslator(issueStates);
    }
}
