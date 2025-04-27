using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;
using TestBucket.Contracts.Fields;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Fields.Handlers;

public record class UpdateTestCaseFieldsRequest(ClaimsPrincipal Principal, IEnumerable<TestCaseField> Fields) : IRequest;

/// <summary>
/// Returns fields (values) for a given entity
/// </summary>
public class UpdateTestCaseFieldsHandler : IRequestHandler<UpdateTestCaseFieldsRequest>
{
    private readonly IFieldManager _fieldManager;

    public UpdateTestCaseFieldsHandler(IFieldManager manager)
    {
        _fieldManager = manager;
    }

    public async ValueTask<Unit> Handle(UpdateTestCaseFieldsRequest request, CancellationToken cancellationToken)
    {
        foreach(var field in request.Fields)
        {
            await _fieldManager.UpsertTestCaseFieldAsync(request.Principal, field);
        }
        return Unit.Value;
    }
}