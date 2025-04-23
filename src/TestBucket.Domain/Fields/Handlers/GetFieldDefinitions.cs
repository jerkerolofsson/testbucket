using Mediator;

using TestBucket.Contracts.Fields;

namespace TestBucket.Domain.Fields;

public record class GetFieldDefinitionsRequest(ClaimsPrincipal Principal, FieldTarget Target, long ProjectId) : IRequest<GetFieldDefinitionsResponse>;
public record class GetFieldDefinitionsResponse(IReadOnlyList<FieldDefinition> Definitions);

/// <summary>
/// Returns field definitions
/// </summary>
public class GetFieldDefinitionsHandler : IRequestHandler<GetFieldDefinitionsRequest, GetFieldDefinitionsResponse>
{
    private readonly IFieldDefinitionManager _fieldDefinitionManager;

    public GetFieldDefinitionsHandler(IFieldDefinitionManager fieldDefinitionManager)
    {
        _fieldDefinitionManager = fieldDefinitionManager;
    }

    public async ValueTask<GetFieldDefinitionsResponse> Handle(GetFieldDefinitionsRequest request, CancellationToken cancellationToken)
    {
        var definitions = await _fieldDefinitionManager.GetDefinitionsAsync(request.Principal, request.ProjectId, request.Target);
        return new GetFieldDefinitionsResponse(definitions);
    }
}
