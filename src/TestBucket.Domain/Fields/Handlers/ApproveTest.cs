using Mediator;

using TestBucket.Domain.States;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.TestCases;

namespace TestBucket.Domain.Fields.Handlers;
public record class ApproveTestCaseRequest(ClaimsPrincipal Principal, TestCase Test, bool IsApproved) : IRequest;

public class ApproveTestCaseRequestHandler(
    IStateService stateService,
    ITestCaseManager testCaseManager,
    IFieldManager fieldManager, IFieldDefinitionManager fieldDefinitionManager) : IRequestHandler<ApproveTestCaseRequest>
{
    public async ValueTask<Unit> Handle(ApproveTestCaseRequest request, CancellationToken cancellationToken)
    {
        var principal = request.Principal;
        var test = request.Test;
        principal.ThrowIfEntityTenantIsDifferent(test);

        // Get definitions
        var definitions = await fieldDefinitionManager.GetDefinitionsAsync(principal, test.TestProjectId, Contracts.Fields.FieldTarget.TestCase);
        var definition = definitions.Where(x=>x.TraitType == Traits.Core.TraitType.Approved).FirstOrDefault();  
        if(definition is null)
        {
            throw new InvalidDataException("No Approved field found");
        }

        var fields = await fieldManager.GetTestCaseFieldsAsync(principal, test.Id, definitions);
        var field = fields.FirstOrDefault(x => x.FieldDefinitionId == definition.Id);
        if (field is null)
        {
            throw new InvalidDataException("Field not found, but definition found");
        }
        field.BooleanValue = request.IsApproved;
        await fieldManager.UpsertTestCaseFieldAsync(principal, field);

        // If it was approved, move it to completed state
        if (test.TestProjectId is not null && request.IsApproved)
        {
            var states = await stateService.GetTestCaseStatesAsync(principal, test.TestProjectId.Value);
            var completedState = states.FirstOrDefault(x => x.MappedState == Contracts.Testing.States.MappedTestState.Completed);
            if(completedState is not null)
            {
                test.State = completedState.Name;
                test.MappedState = completedState.MappedState;
                await testCaseManager.SaveTestCaseAsync(principal, test);
            }
        }

        return Unit.Value;
    }
}
