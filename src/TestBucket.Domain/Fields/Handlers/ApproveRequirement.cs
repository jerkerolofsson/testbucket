using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.Requirements.Models;

namespace TestBucket.Domain.Fields.Handlers;
public record class ApproveRequirementRequest(ClaimsPrincipal Principal, Requirement Requirement, bool IsApproved) : IRequest;

public class ApproveRequirementRequestHandler(IFieldManager fieldManager, IFieldDefinitionManager fieldDefinitionManager) : IRequestHandler<ApproveRequirementRequest>
{
    public async ValueTask<Unit> Handle(ApproveRequirementRequest request, CancellationToken cancellationToken)
    {
        var principal = request.Principal;
        principal.ThrowIfEntityTenantIsDifferent(request.Requirement);

        // Get definitions
        var definitions = await fieldDefinitionManager.GetDefinitionsAsync(principal, request.Requirement.TestProjectId, Contracts.Fields.FieldTarget.Requirement);
        var definition = definitions.Where(x=>x.TraitType == Traits.Core.TraitType.Approved ).FirstOrDefault();  
        if(definition is null)
        {
            throw new InvalidDataException("No Approved field found");
        }

        var fields = await fieldManager.GetRequirementFieldsAsync(principal, request.Requirement.Id, definitions);
        var field = fields.FirstOrDefault(x => x.FieldDefinitionId == definition.Id);
        if (field is null)
        {
            throw new InvalidDataException("Field not found, but definition found");
        }
        field.BooleanValue = request.IsApproved;
        await fieldManager.UpsertRequirementFieldAsync(principal, field);

        return Unit.Value;
    }
}
