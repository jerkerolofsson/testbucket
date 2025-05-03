using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OllamaSharp.Models.Chat;
using OneOf.Types;

using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Duplication;
internal static class TestCaseRunDuplicationExtensions
{
    /// <summary>
    /// Creates a copy of the test case, not including fields
    /// </summary>
    /// <param name="testCase"></param>
    /// <returns></returns>
    public static TestCaseRun Duplicate(this TestCaseRun testCase, bool includeResultFields = false)
    {
        var copy = new TestCaseRun
        {
            Name = testCase.Name,
            TestProjectId = testCase.TestProjectId,
            TeamId = testCase.TeamId,
            TestCaseId = testCase.TestCaseId,
            AssignedToUserId = testCase.AssignedToUserId,
            AssignedToUserName = testCase.AssignedToUserName,
           
        };

        if(includeResultFields)
        {
            copy.Result = testCase.Result;
            copy.State = testCase.State;
            copy.MappedState = testCase.MappedState;
            copy.Message = testCase.Message;
            copy.CallStack = testCase.Message;
            copy.Duration = testCase.Duration;
            copy.SystemErr = testCase.SystemErr;
            copy.SystemOut = testCase.SystemOut;
        }

        return copy;
    }
}
