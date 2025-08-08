using Mediator;

using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Fields.Handlers;
public record class ApproveTestCaseRequest(ClaimsPrincipal Principal, TestCase Test, bool IsApproved) : IRequest;

public class ApproveTestCaseRequestHandler(IFieldManager fieldManager, IFieldDefinitionManager fieldDefinitionManager) : IRequestHandler<ApproveTestCaseRequest>
{
    public async ValueTask<Unit> Handle(ApproveTestCaseRequest request, CancellationToken cancellationToken)
    {
        var principal = request.Principal;
        principal.ThrowIfEntityTenantIsDifferent(request.Test);

        // Get definitions
        var definitions = await fieldDefinitionManager.GetDefinitionsAsync(principal, request.Test.TestProjectId, Contracts.Fields.FieldTarget.TestCase);
        var definition = definitions.Where(x=>x.TraitType == Traits.Core.TraitType.Approved).FirstOrDefault();  
        if(definition is null)
        {
            throw new InvalidDataException("No Approved field found");
        }

        var fields = await fieldManager.GetTestCaseFieldsAsync(principal, request.Test.Id, definitions);
        var field = fields.FirstOrDefault(x => x.FieldDefinitionId == definition.Id);
        if (field is null)
        {
            throw new InvalidDataException("Field not found, but definition found");
        }
        field.BooleanValue = request.IsApproved;
        await fieldManager.UpsertTestCaseFieldAsync(principal, field);

        return Unit.Value;
    }
}
