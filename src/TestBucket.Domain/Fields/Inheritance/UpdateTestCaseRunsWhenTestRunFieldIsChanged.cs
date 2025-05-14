using Mediator;

using TestBucket.Contracts.Fields;
using TestBucket.Domain.Fields.Events;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Testing.TestRuns;
using TestBucket.Domain.Testing.TestRuns.Search;

namespace TestBucket.Domain.Fields.Inheritance;
public class UpdateTestCaseRunsWhenTestRunFieldIsChanged : INotificationHandler<TestRunFieldChangedNotification>
{
    private readonly IFieldManager _fieldManager;
    private readonly IFieldDefinitionManager _fieldDefinitionManager;
    private readonly ITestRunManager _testRunManager;

    public UpdateTestCaseRunsWhenTestRunFieldIsChanged(
        IFieldManager fieldManager, 
        ITestRunManager testRunManager, 
        IFieldDefinitionManager fieldDefinitionManager)
    {
        _fieldManager = fieldManager;
        _testRunManager = testRunManager;
        _fieldDefinitionManager = fieldDefinitionManager;
    }

    public async ValueTask Handle(TestRunFieldChangedNotification notification, CancellationToken cancellationToken)
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
                var result = await _testRunManager.SearchTestCaseRunsAsync(principal, new SearchTestCaseRunQuery { TestRunId = notification.Field.TestRunId, Offset = offset, Count = pageSize });
                
                // Loop over all test case runs
                foreach(var testCaseRun in result.Items)
                {
                    var fields = await _fieldManager.GetTestCaseRunFieldsAsync(principal, notification.Field.TestRunId, testCaseRun.Id, fieldDefinitions);
                    var testCaseRunField = fields.Where(x=>x.FieldDefinitionId == field.FieldDefinitionId).FirstOrDefault();
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
