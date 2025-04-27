using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Contracts.Fields;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.TestRuns;
using TestBucket.Formats.Dtos;

namespace TestBucket.Domain.Testing.Services.Export;

public record class ExportTestCaseRunRequest(ClaimsPrincipal Principal, long Id) : IRequest<TestCaseRunDto>;

public class ExportTestCaseRunHandler : IRequestHandler<ExportTestCaseRunRequest, TestCaseRunDto>
{
    private readonly ITestRunManager _testRunManager;
    private readonly IFieldDefinitionManager _fieldDefinitionManager;

    public ExportTestCaseRunHandler(ITestRunManager testRunManager, IFieldDefinitionManager fieldDefinitionManager)
    {
        _testRunManager = testRunManager;
        _fieldDefinitionManager = fieldDefinitionManager;
    }

    public async ValueTask<TestCaseRunDto> Handle(ExportTestCaseRunRequest request, CancellationToken cancellationToken)
    {
        var principal = request.Principal;
        TestCaseRun? testCaseRun = await _testRunManager.GetTestCaseRunByIdAsync(principal, request.Id);
        if (testCaseRun is null)
        {
            throw new FileNotFoundException();
        }
        var fieldDefinitions = await _fieldDefinitionManager.GetDefinitionsAsync(principal, testCaseRun.TestProjectId, FieldTarget.TestCaseRun);

        var dto = new TestCaseRunDto()
        {
            Name = testCaseRun.Name,
            CreatedTime = testCaseRun.Created,
            Team = testCaseRun.Team?.Slug,
            Project = testCaseRun.TestProject?.Slug,
        };

        if (testCaseRun.TestCaseRunFields is not null)
        {
            foreach (var field in testCaseRun.TestCaseRunFields)
            {
                var fieldDefinition = fieldDefinitions.Where(x => x.Id == field.FieldDefinitionId).FirstOrDefault();
                if (fieldDefinition is not null)
                {
                    dto.Traits.Add(new TestTrait
                    {
                        Name = fieldDefinition.Trait ?? fieldDefinition.Name,
                        Type = fieldDefinition.TraitType,
                        Value = field.GetValueAsString()
                    });
                }
            }
        }


        return dto;
    }
}