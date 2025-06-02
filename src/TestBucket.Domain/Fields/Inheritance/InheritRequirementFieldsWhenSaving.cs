using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Contracts.Fields;
using TestBucket.Domain.Requirements;
using TestBucket.Domain.Requirements.Events;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.TestRuns.Events;

namespace TestBucket.Domain.Fields.Inheritance;
internal class InheritRequirementFieldsWhenSaving : 
    INotificationHandler<RequirementUpdatedEvent>,
    INotificationHandler<RequirementCreatedEvent>
{

    private readonly IRequirementManager _manager;
    private readonly IFieldDefinitionManager _fieldDefinitionManager;
    private readonly IFieldManager _fieldManager;
    public InheritRequirementFieldsWhenSaving(IRequirementManager manager, IFieldDefinitionManager fieldDefinitionManager, IFieldManager fieldManager)
    {
        _manager = manager;
        _fieldDefinitionManager = fieldDefinitionManager;
        _fieldManager = fieldManager;
    }

    public async ValueTask Handle(RequirementUpdatedEvent notification, CancellationToken cancellationToken)
    {
        await HandleAsync(notification.Principal, notification.Requirement);
    }

    public async ValueTask Handle(RequirementCreatedEvent notification, CancellationToken cancellationToken)
    {
        await HandleAsync(notification.Principal, notification.Requirement);
    }

    private async Task HandleAsync(ClaimsPrincipal principal, Requirement requirement)
    {
        var fieldDefinitions = await _fieldDefinitionManager.GetDefinitionsAsync(principal, requirement.TestProjectId, FieldTarget.Requirement);
        var requirementFields = await _fieldManager.GetRequirementFieldsAsync(principal, requirement.Id, fieldDefinitions);

        await InheritFieldFromParentRequirementAsync(principal, requirement, fieldDefinitions, requirementFields);
    }
    internal async Task InheritFieldFromParentRequirementAsync(
        ClaimsPrincipal principal, 
        Requirement requirement, 
        IReadOnlyList<FieldDefinition> fieldDefinitions, 
        IReadOnlyList<RequirementField> requirementFields)
    {
        if (requirement.ParentRequirementId is not null)
        {
            var parentRequirement = await _manager.GetRequirementByIdAsync(principal, requirement.ParentRequirementId.Value);

            if (parentRequirement is not null)
            {
                var parentFields = await _fieldManager.GetRequirementFieldsAsync(principal, parentRequirement.Id, fieldDefinitions);

                // Add any missing fields or inherited fields
                foreach (var field in requirementFields)
                {
                    field.FieldDefinition ??= fieldDefinitions.Where(x => x.Id == field.FieldDefinitionId).FirstOrDefault();
                    if(field.FieldDefinition is null || field.FieldDefinition.Inherit == false)
                    {
                        continue;
                    }

                    if (!field.HasValue() || field.Inherited == true)
                    {
                        var parentField = parentFields.Where(x => x.FieldDefinitionId == field.FieldDefinitionId).FirstOrDefault();
                        if (parentField is not null)
                        {
                            parentField.CopyTo(field);
                            field.Inherited = true;
                            await _fieldManager.UpsertRequirementFieldAsync(principal, field);
                        }
                    }
                }
            }
        }
    }
}
