using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.Fields;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Traits.Core;

namespace TestBucket.Domain.Requirements.Fields;
public record class SetRequirementFieldRequest(ClaimsPrincipal Principal, Requirement Requirement, TraitType Type, string? value) : IRequest;

public class SetRequirementFieldHandler : IRequestHandler<SetRequirementFieldRequest>
{
    private readonly IFieldDefinitionManager _fieldDefinitionManager;
    private readonly IFieldManager _fieldManager;

    public SetRequirementFieldHandler(IFieldManager fieldManager, IFieldDefinitionManager fieldDefinitionManager)
    {
        _fieldManager = fieldManager;
        _fieldDefinitionManager = fieldDefinitionManager;
    }

    public async ValueTask<Unit> Handle(SetRequirementFieldRequest request, CancellationToken cancellationToken)
    {
        var principal = request.Principal;
        principal.ThrowIfNoPermission(PermissionEntityType.Requirement, PermissionLevel.Write);

        var requirement = request.Requirement;
        var fields = await _fieldDefinitionManager.GetDefinitionsAsync(principal, requirement.TestProjectId, Contracts.Fields.FieldTarget.Requirement);
        var field = fields.Where(x => x.TraitType == request.Type).FirstOrDefault();
        if(field is null)
        {
            return Unit.Value;
        }


        if (requirement.RequirementFields is not null)
        {
            var requirementField = requirement.RequirementFields.Where(x => x.FieldDefinitionId == field.Id).FirstOrDefault();
            if(requirementField is not null)
            {
                requirementField.StringValue = request.value;
                await _fieldManager.UpsertRequirementFieldAsync(principal, requirementField);
                return Unit.Value;
            }
        }

        var newField = new RequirementField { FieldDefinitionId = field.Id, RequirementId = requirement.Id };
        newField.StringValue = request.value;
        await _fieldManager.UpsertRequirementFieldAsync(principal, newField);

        if(requirement.RequirementFields is not null)
        {
            newField.FieldDefinition = field;
            requirement.RequirementFields = [.. requirement.RequirementFields, newField];
        }

        return Unit.Value;
    }
}
