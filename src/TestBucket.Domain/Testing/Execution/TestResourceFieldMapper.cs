using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.TestResources;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Execution;
public static class TestResourceFieldMapper
{
    /// <summary>
    /// Reads variables from the resources, extracting
    /// </summary>
    /// <param name="fields"></param>
    /// <param name="testCaseRun"></param>
    /// <param name="resources"></param>
    /// <returns></returns>
    internal static List<TestCaseRunField> MapResourcesToFields(IReadOnlyList<FieldDefinition> fields, TestCaseRun testCaseRun, List<TestResourceDto> resources)
    {
        List<TestCaseRunField> result = [];
        var alreadyMappedFieldDefinitions = new HashSet<long>();
        foreach (var resource in resources)
        {
            if (resource.Variables is not null)
            {
                foreach (var variable in resource.Variables)
                {
                    var fieldDefinition = fields.FirstOrDefault(x => x.Trait == variable.Key);
                    if(fieldDefinition is not null)
                    {
                        // Only extract one field definition for each test run.
                        // If there are multiple resources then use the first one
                        // For example if we have two android phones, we will read the software version from the first one
                        // which should be the DUT..
                        if (alreadyMappedFieldDefinitions.Contains(fieldDefinition.Id))
                        {
                            break;
                        }
                        

                        var field = new TestCaseRunField
                        {
                            TestRunId = testCaseRun.TestRunId,
                            TestCaseRunId = testCaseRun.Id,
                            FieldDefinitionId = fieldDefinition.Id,
                        };

                        if (FieldValueConverter.TryAssignValue(fieldDefinition, field, [variable.Value]))
                        {
                            result.Add(field);
                            alreadyMappedFieldDefinitions.Add(fieldDefinition.Id);
                        }
                    }
                }
            }
        }
        return result;
    }
}
