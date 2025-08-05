using Mediator;

using TestBucket.Contracts.Fields;
using TestBucket.Formats.Dtos;

namespace TestBucket.Domain.Fields.Handlers;
public record class ImportTraitRequest(ClaimsPrincipal Principal, long ProjectId, TestTrait Trait, FieldTarget Target) : IRequest<FieldDefinition>;

/// <summary>
/// Imports a trait, adding it as a FieldDefinition
/// </summary>
public class ImportTraitHandler : IRequestHandler<ImportTraitRequest, FieldDefinition>
{
    private readonly IFieldDefinitionManager _fieldDefinitionManager;
    public ImportTraitHandler(IFieldDefinitionManager fieldDefinitionManager)
    {
        _fieldDefinitionManager = fieldDefinitionManager;
    }
    public async ValueTask<FieldDefinition> Handle(ImportTraitRequest request, CancellationToken cancellationToken)
    {
        var fieldDefinitions = await _fieldDefinitionManager.GetDefinitionsAsync(request.Principal, request.ProjectId);

        // Get by trait first, and if not found get by name
        var fieldDefinition = fieldDefinitions.Where(x => x.TraitType == request.Trait.Type).FirstOrDefault() ??
            fieldDefinitions.Where(x => x.Name == request.Trait.Name).FirstOrDefault();
        if(fieldDefinition is null)
        {
            // Add a new field
            fieldDefinition = new FieldDefinition { Name = request.Trait.Name, TraitType = request.Trait.Type, TestProjectId = request.ProjectId, IsVisible = true, Target = request.Target, Type = FieldType.String };
            await _fieldDefinitionManager.AddAsync(request.Principal, fieldDefinition);
            return fieldDefinition;
        }
        else
        {
            // Update the field if the target is not matching
            if((fieldDefinition.Target&request.Target) != request.Target)
            {
                fieldDefinition.Target |= request.Target;
                await _fieldDefinitionManager.UpdateAsync(request.Principal, fieldDefinition);
            }
            return fieldDefinition;
        }

    }
}