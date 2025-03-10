using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using TestBucket.Formats.Ctrf.Models;
using TestBucket.Formats.Shared;

namespace TestBucket.Formats.Ctrf
{
    public abstract class CtrfSerializer<TEnvironmentExtra, TTestsExtra> : ITestResultSerializer
        where TEnvironmentExtra : EnvironmentExtra
        where TTestsExtra : TestExtra
    {
        public TestRunDto Deserialize(string text)
        {
            var report = JsonSerializer.Deserialize<CtrfReport<TEnvironmentExtra, TTestsExtra>>(text);

            var name = report?.Results?.Environment?.ReportName ?? "";

            var run = new TestRunDto() { Name = name };

            if(report?.Results?.Tests is not null)
            {
                foreach (var test in report.Results.Tests)
                {
                    var testDto = new TestCaseRunDto()
                    {
                        Name = test.Name,
                        Result = GetTestResultFromCtrfStatus(test.Status),
                        Browser = test.Browser,
                        Message = test.Message,
                        TestCategory = test.Type,
                        
                        Module = test.FilePath,
                        TestFilePath = test.FilePath

                    };

                    if(test.Screenshot is not null && test.Screenshot.StartsWith("data:"))
                    {
                        // "data:image/png;base64,aGVsbG93b3JsZA=="
                        var attachment = DataUriParser.ParseDataUri(test.Screenshot);
                        attachment.IsScreenshot = true;
                        testDto.Attachments.Add(attachment);
                    }

                    ParseTestExtra(test, testDto);
                    //run.Tests.Add(testDto);

                    if(test.Tags is not null)
                    {
                        foreach(var tag in test.Tags)
                        {
                            testDto.AddTag(tag);
                        }
                    }

                    
                }
            }

            return run;
        }

        protected virtual void ParseTestExtra(CtrfTest<TTestsExtra> test, TestCaseRunDto testDto)
        {
        }

        private TestResult GetTestResultFromCtrfStatus(string status)
        {
            return status switch
            {
                "passed" => TestResult.Passed,
                "failed" => TestResult.Failed,
                "pending" => TestResult.NoRun,
                "skipped" => TestResult.Skipped,
                "other" => TestResult.Other,

                // Not in spec
                "error" => TestResult.Error,
                "blocked" => TestResult.Blocked,
                "hang" => TestResult.Hang,
                "crashed" => TestResult.Crashed,

                _ => TestResult.Other
            };
        }

        public string Serialize(TestRunDto testRun)
        {
            throw new NotImplementedException();
        }
    }
}
