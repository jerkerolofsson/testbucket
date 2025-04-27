using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Contracts.Fields;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Testing.TestRuns;
using TestBucket.Formats.Dtos;

namespace TestBucket.Domain.Testing.Services.Export;

public record class ExportRunRequest(ClaimsPrincipal Principal, long Id) : IRequest<TestRunDto>;

public class ExportRunHandler : IRequestHandler<ExportRunRequest, TestRunDto>
{
    private readonly ITestRunManager _testRunManager;
    private readonly IFieldDefinitionManager _fieldDefinitionManager;

    public ExportRunHandler(ITestRunManager testRunManager, IFieldDefinitionManager fieldDefinitionManager)
    {
        _testRunManager = testRunManager;
        _fieldDefinitionManager = fieldDefinitionManager;
    }

    public async ValueTask<TestRunDto> Handle(ExportRunRequest request, CancellationToken cancellationToken)
    {
        var principal = request.Principal;
        var testRun = await _testRunManager.GetTestRunByIdAsync(principal, request.Id);
        if (testRun is null)
        {
            throw new FileNotFoundException();
        }
        var testRunFieldDefinitions = await _fieldDefinitionManager.GetDefinitionsAsync(principal, testRun.TestProjectId, FieldTarget.TestRun);

        var dto = new TestRunDto()
        {
            Name = testRun.Name,
            CreatedTime = testRun.Created,
            Team = testRun.Team?.Slug,
            Project =testRun.TestProject?.Slug,
            Slug = testRun.Slug,
        };

        if (testRun.TestRunFields is not null)
        {
            foreach (var field in testRun.TestRunFields)
            {
                var fieldDefinition = testRunFieldDefinitions.Where(x => x.Id == field.FieldDefinitionId).FirstOrDefault();
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