using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Services;

/// <summary>
/// Assigns fields to test case runs
/// </summary>
internal class TestCaseRunFieldHelper
{

    public static void BuildInheritedFields(TestRun testRun, TestCase testCase, IReadOnlyList<FieldDefinition> testCaseFieldDefinitions, IReadOnlyList<FieldDefinition> testRunFieldDefinitions, TestCaseRun testCaseRun, List<TestCaseRunField> fields)
    {
        // Add inherited fields from test run
        if (testRun.TestRunFields is not null)
        {
            foreach (var field in testRun.TestRunFields)
            {
                var fieldDefinition = testRunFieldDefinitions.Where(x => x.Id == field.FieldDefinitionId).FirstOrDefault();
                if (fieldDefinition is not null &&
                    fieldDefinition.Inherit &&
                    (fieldDefinition.Target & FieldTarget.TestCaseRun) == FieldTarget.TestCaseRun)
                {
                    var newField = new TestCaseRunField
                    {
                        FieldDefinitionId = fieldDefinition.Id,
                        TestCaseRunId = testCaseRun.Id,
                        TestRunId = testRun.Id
                    };
                    field.CopyTo(newField);

                    // Replace it
                    fields.RemoveAll(x => x.FieldDefinitionId == newField.FieldDefinitionId);
                    fields.Add(newField);
                }
            }
        }

        // Add inherited fields from test case
        if (testCase.TestCaseFields is not null)
        {
            foreach (var field in testCase.TestCaseFields)
            {
                var fieldDefinition = testCaseFieldDefinitions.Where(x => x.Id == field.FieldDefinitionId).FirstOrDefault();
                if (fieldDefinition is not null &&
                    fieldDefinition.Inherit &&
                    (fieldDefinition.Target & FieldTarget.TestCaseRun) == FieldTarget.TestCaseRun)
                {
                    var newField = new TestCaseRunField
                    {
                        FieldDefinitionId = fieldDefinition.Id,
                        TestCaseRunId = testCaseRun.Id,
                        TestRunId = testRun.Id
                    };
                    field.CopyTo(newField);

                    // Replace it
                    fields.RemoveAll(x => x.FieldDefinitionId == newField.FieldDefinitionId);
                    fields.Add(newField);
                }
            }
        }
    }
}
