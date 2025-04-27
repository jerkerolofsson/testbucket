using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;
using TestBucket.Contracts.Fields;

using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.TestRuns.Events;

namespace TestBucket.Domain.Fields.Inheritance;

/// <summary>
/// Updates a test case run field when it is saved, by copying the value from test case or test run
/// </summary>
public class InheritTestCaseRunFieldsWhenSaving : INotificationHandler<TestCaseRunSavedNotification>
{
    private readonly IFieldDefinitionManager _fieldDefinitionManager;
    private readonly IFieldManager _fieldManager;

    public InheritTestCaseRunFieldsWhenSaving(IFieldDefinitionManager fieldDefinitionManager, IFieldManager fieldManager)
    {
        _fieldDefinitionManager = fieldDefinitionManager;
        _fieldManager = fieldManager;
    }

    public async ValueTask Handle(TestCaseRunSavedNotification notification, CancellationToken cancellationToken)
    {
        var principal = notification.Principal;
        var testCaseRun = notification.TestCaseRun;

        var runFieldDefinitions = await _fieldDefinitionManager.GetDefinitionsAsync(principal, testCaseRun.TestProjectId, FieldTarget.TestRun);
        var testCaseDefinitions = await _fieldDefinitionManager.GetDefinitionsAsync(principal, testCaseRun.TestProjectId, FieldTarget.TestRun);
        var testCaseRunDefinitions = await _fieldDefinitionManager.GetDefinitionsAsync(principal, testCaseRun.TestProjectId, FieldTarget.TestCaseRun);

        var testRunFields = await _fieldManager.GetTestRunFieldsAsync(principal, testCaseRun.TestRunId, runFieldDefinitions);
        var testCaseFields = await _fieldManager.GetTestCaseFieldsAsync(principal, testCaseRun.TestCaseId, testCaseDefinitions);
        var testCaseRunFields = await _fieldManager.GetTestCaseRunFieldsAsync(principal, testCaseRun.TestRunId, testCaseRun.Id, testCaseRunDefinitions);

        // Add any missing fields or inherited fields
        foreach(var field in testCaseRunFields)
        {
            if(!field.HasValue() || field.Inherited == true)
            {
                await UpdateFieldAsync(principal, field, testCaseFields, testRunFields);
            }
        }
    }

    private async Task UpdateFieldAsync(
        ClaimsPrincipal principal,
        TestCaseRunField field, 
        IReadOnlyList<TestCaseField> testCaseFields, 
        IReadOnlyList<TestRunField> testRunFields)
    {
        var testCaseField = testCaseFields.Where(x => x.FieldDefinitionId == field.FieldDefinitionId).FirstOrDefault();
        var testRunField = testRunFields.Where(x => x.FieldDefinitionId == field.FieldDefinitionId).FirstOrDefault();

        var changed = false;
        if (testCaseField is not null && testCaseField.HasValue())
        {
            changed = testCaseField.CopyTo(field);
        }
        if (testRunField is not null && testRunField.HasValue())
        {
            changed = testRunField.CopyTo(field);
        }

        if (changed)
        {
            field.Inherited = true;
            await _fieldManager.UpsertTestCaseRunFieldAsync(principal, field);
        }
    }
}
