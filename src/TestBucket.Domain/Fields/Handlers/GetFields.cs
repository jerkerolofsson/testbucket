using Mediator;

using TestBucket.Contracts.Fields;

namespace TestBucket.Domain.Fields;

public record class GetFieldsRequest(ClaimsPrincipal Principal, FieldTarget Target, long ProjectId, long EntityId) : IRequest<GetFieldsResponse>;
public record class GetFieldsResponse(IReadOnlyList<FieldValue> Fields);

/// <summary>
/// Returns fields (values) for a given entity
/// </summary>
public class GetFieldsHandler : IRequestHandler<GetFieldsRequest, GetFieldsResponse>
{
    private readonly IFieldManager _fieldManager;
    private readonly IFieldDefinitionManager _fieldDefinitionManager;

    public GetFieldsHandler(IFieldManager manager, IFieldDefinitionManager fieldDefinitionManager)
    {
        _fieldManager = manager;
        _fieldDefinitionManager = fieldDefinitionManager;
    }

    public async ValueTask<GetFieldsResponse> Handle(GetFieldsRequest request, CancellationToken cancellationToken)
    {
        var definitions = await _fieldDefinitionManager.GetDefinitionsAsync(request.Principal, request.ProjectId, request.Target);

        switch (request.Target)
        {
            case FieldTarget.TestCase:
                return new GetFieldsResponse(await _fieldManager.GetTestCaseFieldsAsync(request.Principal, request.EntityId, definitions));

            case FieldTarget.TestRun:
                return new GetFieldsResponse(await _fieldManager.GetTestRunFieldsAsync(request.Principal, request.EntityId, definitions));

            case FieldTarget.Issue:
                return new GetFieldsResponse(await _fieldManager.GetIssueFieldsAsync(request.Principal, request.EntityId, definitions));

            default:
                throw new NotImplementedException();
        }

    }
}