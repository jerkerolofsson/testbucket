using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Contracts.Fields;
using TestBucket.Domain.Fields.Handlers;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Duplication;
public record class DuplicateTestCaseRequest(ClaimsPrincipal Principal, TestCase TestCase) : IRequest<TestCase>;

public class DuplicateTestCaseHandler : IRequestHandler<DuplicateTestCaseRequest, TestCase>
{
    private readonly IMediator _mediator;
    private readonly ITestCaseManager _testCaseManager;

    public DuplicateTestCaseHandler(IMediator mediator, ITestCaseManager testCaseManager)
    {
        _mediator = mediator;
        _testCaseManager = testCaseManager;
    }

    public async ValueTask<TestCase> Handle(DuplicateTestCaseRequest request, CancellationToken cancellationToken)
    {
        var principal = request.Principal;
        var testCase = request.TestCase;
        ArgumentNullException.ThrowIfNull(testCase.TestProjectId);
        principal.ThrowIfEntityTenantIsDifferent(testCase);
        principal.ThrowIfNoPermission(PermissionEntityType.TestCase, PermissionLevel.Write);

        var copy = testCase.Duplicate();
        await _testCaseManager.AddTestCaseAsync(principal, copy);
        await CopyFieldsAsync(principal, testCase, copy);

        return testCase;

        async Task CopyFieldsAsync(ClaimsPrincipal principal, TestCase testCase, TestCase copy)
        {
            List<TestCaseField> testCaseFields = new();
            var fields = await _mediator.Send(new GetFieldsRequest(principal, FieldTarget.TestCase, testCase.TestProjectId!.Value, testCase.Id));
            foreach (var field in fields.Fields)
            {
                var testCaseField = new TestCaseField { FieldDefinitionId = field.FieldDefinitionId, TestCaseId = copy.Id };
                field.CopyTo(testCaseField);
                testCaseFields.Add(testCaseField);
            }

            await _mediator.Send(new UpdateTestCaseFieldsRequest(principal, testCaseFields));
        }
    }
}
