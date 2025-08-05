using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;

using TestBucket.Contracts.Fields;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Testing.Models;
using TestBucket.Formats.Dtos;

namespace TestBucket.Domain.Testing.Services.Import;
public class TestCaseRunFieldImporter
{
    private readonly ClaimsPrincipal _principal;
    private readonly IFieldManager _fieldManager;
    private readonly IFieldDefinitionManager _fieldDefinitionManager;

    public TestCaseRunFieldImporter(ClaimsPrincipal principal, IFieldManager fieldManager, IFieldDefinitionManager fieldDefinitionManager)
    {
        _principal = principal;
        _fieldManager = fieldManager;
        _fieldDefinitionManager = fieldDefinitionManager;
    }

    public async Task ImportAsync(TestCaseRunDto source, TestCaseRun destination)
    {
        // Set fields from DTO (overriding any inherited values)
        var fieldDefinitions = await _fieldDefinitionManager.GetDefinitionsAsync(_principal, destination.TestProjectId, FieldTarget.TestCaseRun);
        var fields = await _fieldManager.GetTestCaseRunFieldsAsync(_principal, destination.TestRunId, destination.Id, fieldDefinitions);

        foreach (var field in fields)
        {
            var fieldDefinition = field.FieldDefinition;
            if (fieldDefinition is null)
            {
                continue;
            }
            var values = source.Traits.Where(x => x.Type == fieldDefinition.TraitType).Select(x => x.Value).ToArray();
            if (FieldValueConverter.TryAssignValue(fieldDefinition, field, values))
            {
                await _fieldManager.UpsertTestCaseRunFieldAsync(_principal, field);
            }
        }
    }
}
