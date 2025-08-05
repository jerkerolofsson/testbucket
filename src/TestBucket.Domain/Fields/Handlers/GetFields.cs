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
                request.Principal.ThrowIfNoPermission(PermissionEntityType.TestCase, PermissionLevel.Read);
                return new GetFieldsResponse(await _fieldManager.GetTestCaseFieldsAsync(request.Principal, request.EntityId, definitions));

            case FieldTarget.TestRun:
                request.Principal.ThrowIfNoPermission(PermissionEntityType.TestRun, PermissionLevel.Read);
                return new GetFieldsResponse(await _fieldManager.GetTestRunFieldsAsync(request.Principal, request.EntityId, definitions));

            case FieldTarget.Requirement:
                request.Principal.ThrowIfNoPermission(PermissionEntityType.Requirement, PermissionLevel.Read);
                return new GetFieldsResponse(await _fieldManager.GetRequirementFieldsAsync(request.Principal, request.EntityId, definitions));

            case FieldTarget.Issue:
                request.Principal.ThrowIfNoPermission(PermissionEntityType.Issue, PermissionLevel.Read);
                return new GetFieldsResponse(await _fieldManager.GetIssueFieldsAsync(request.Principal, request.EntityId, definitions));

            default:
                throw new NotImplementedException();
        }

    }
}