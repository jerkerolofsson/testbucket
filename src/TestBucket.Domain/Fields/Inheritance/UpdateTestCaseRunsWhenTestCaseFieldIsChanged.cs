using Mediator;

using TestBucket.Contracts.Fields;
using TestBucket.Domain.Fields.Events;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.TestRuns;
using TestBucket.Domain.Testing.TestRuns.Search;

namespace TestBucket.Domain.Fields.Inheritance;
public class UpdateTestCaseRunsWhenTestCaseFieldIsChanged : INotificationHandler<TestCaseFieldChangedNotification>
{
    private readonly IFieldManager _fieldManager;
    private readonly IFieldDefinitionManager _fieldDefinitionManager;
    private readonly ITestRunManager _testRunManager;

    public UpdateTestCaseRunsWhenTestCaseFieldIsChanged(
        IFieldManager fieldManager, 
        ITestRunManager testRunManager, 
        IFieldDefinitionManager fieldDefinitionManager)
    {
        _fieldManager = fieldManager;
        _testRunManager = testRunManager;
        _fieldDefinitionManager = fieldDefinitionManager;
    }

    public async ValueTask Handle(TestCaseFieldChangedNotification notification, CancellationToken cancellationToken)
    {
        if(notification.Field?.FieldDefinition?.Inherit == true && notification.Field.HasValue())
        {
            var principal = notification.Principal;
            var field = notification.Field;

            var fieldDefinitions = await _fieldDefinitionManager.GetDefinitionsAsync(principal, field.FieldDefinition.TestProjectId, FieldTarget.TestCaseRun);

            int pageSize = 100;
            int offset = 0;
            while(true)
            {
                var result = await _testRunManager.SearchTestCaseRunsAsync(principal, 
                    new SearchTestCaseRunQuery { TestCaseId = notification.Field.TestCaseId, Offset = offset, Count = pageSize });
                
                // Loop over all test case runs
                foreach(var testCaseRun in result.Items)
                {
                    var fields = testCaseRun.TestCaseRunFields ?? await _fieldManager.GetTestCaseRunFieldsAsync(principal, testCaseRun.TestRunId, testCaseRun.Id, fieldDefinitions);

                    // Find the field matchin the updated test case field
                    var testCaseRunField = fields.Where(x=>x.FieldDefinitionId == field.FieldDefinitionId).FirstOrDefault();
                    if (testCaseRunField?.FieldDefinition is null)
                    {
                        continue;
                    }
                    if (testCaseRunField.Inherited == false)
                    {
                        // Don't update a value manually set by a user
                        continue;
                    }

                    // If the run defines this field, ignore the change on the test case
                    var testRunField = (await _fieldManager.GetTestRunFieldsAsync(principal, testCaseRun.TestRunId, [])).Where(x=>x.FieldDefinitionId == field.FieldDefinitionId).FirstOrDefault();
                    if(testRunField is not null && testRunField.HasValue())
                    {
                        continue;
                    }

                    // Update the value 
                    if(testCaseRunField is not null && testCaseRunField.Inherited != false)
                    {
                        if(field.CopyTo(testCaseRunField))
                        {
                            await _fieldManager.UpsertTestCaseRunFieldAsync(principal, testCaseRunField);
                        }
                    }
                }

                offset += pageSize;
                if(result.Items.Length != pageSize)
                {
                    break;
                }
            }
        }

    }
}
