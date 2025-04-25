using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Duplication;
internal static class TestCaseDuplicationExtensions
{
    /// <summary>
    /// Creates a copy of the test case, not including fields
    /// </summary>
    /// <param name="testCase"></param>
    /// <returns></returns>
    public static TestCase Duplicate(this TestCase testCase)
    {
        return new TestCase
        {
            Name = testCase.Name + " copy",
            Description = testCase.Description,

            TestSuiteId = testCase.TestSuiteId,
            TestProjectId = testCase.TestProjectId,
            TestSuiteFolderId = testCase.TestSuiteFolderId,
            TeamId = testCase.TeamId,

            TestParameters = testCase.TestParameters,
            ExecutionType = testCase.ExecutionType,
            ScriptType = testCase.ScriptType,
            RunnerLanguage = testCase.RunnerLanguage,
        };
    }
}
