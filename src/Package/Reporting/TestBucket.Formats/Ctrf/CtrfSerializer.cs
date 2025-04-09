using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using TestBucket.Formats.Ctrf.Models;
using TestBucket.Formats.Shared;
using TestBucket.Traits.Core;

namespace TestBucket.Formats.Ctrf
{
    public abstract class CtrfSerializer<TResultsExtra, TEnvironmentExtra, TTestsExtra> : ITestResultSerializer
        where TResultsExtra : ResultsExtra, new()
        where TEnvironmentExtra : EnvironmentExtra
        where TTestsExtra : TestExtra
    {
        protected static readonly HashSet<TraitType> _nativeAttributes =
        [
            TraitType.TestId,
            TraitType.Name,
            TraitType.TestResult,
            TraitType.Line,
            TraitType.Duration,
            TraitType.ClassName,

            TraitType.SystemOut,
            TraitType.SystemErr,

            TraitType.FailureMessage,
            TraitType.CallStack,
            TraitType.FailureType,

            TraitType.SoftwareVersion,
        ];

        /// <summary>
        /// Returns the suite identifier 
        /// </summary>
        /// <param name="test"></param>
        /// <returns></returns>
        public virtual string GetSuiteIdentifier(CtrfTest<TTestsExtra> test)
        {
            return test.Suite ?? "unknown";
        }

        public TestRunDto Deserialize(string json)
        {
            var options = new JsonSerializerOptions {  PropertyNamingPolicy = JsonNamingPolicy.CamelCase, AllowTrailingCommas = true };
            var report = JsonSerializer.Deserialize<CtrfReport<TResultsExtra, TEnvironmentExtra, TTestsExtra>>(json);
            if(report is null)
            {
                return new TestRunDto();
            }

            var name = report.Results?.Environment?.ReportName ?? "";

            var run = new TestRunDto() { Name = name };
            if(report.Results?.Summary is not null)
            {
                run.StartedTime = ParseEpochs(report.Results.Summary.Start);
                run.EndedTime = ParseEpochs(report.Results.Summary.Stop);
            }
            if (report.Results?.Environment?.AppVersion is not null)
            {
                run.SoftwareVersion = report.Results?.Environment?.AppVersion;
            }

            // CTRF does not have a hierarchy for suites, each test reference a test suite
            Dictionary<string, TestSuiteRunDto> suites = new();

            if(report?.Results?.Tests is not null)
            {
                foreach (var test in report.Results.Tests)
                {
                    var suiteIdentifier = test.Suite ?? "unknown";
                    TestSuiteRunDto suite = GetOrCreateSuiteByName(suiteIdentifier, suites, report, run);

                    var testDto = new TestCaseRunDto()
                    {
                        Name = test.Name,
                        Result = CtrfResultMapping.GetTestResultFromCtrfStatus(test.Status),
                        Browser = test.Browser,
                        Message = test.Message,
                        TestCategory = test.Type,
                        
                        Module = test.FilePath,
                        TestFilePath = test.FilePath
                    };

                    if(!string.IsNullOrEmpty(testDto.Assembly))
                    {
                        suite.Assembly = testDto.Assembly;
                    }

                    suite.Tests.Add(testDto);

                    if(test.Screenshot is not null && test.Screenshot.StartsWith("data:"))
                    {
                        // "data:image/png;base64,aGVsbG93b3JsZA=="
                        var attachment = DataUriParser.ParseDataUri(test.Screenshot);
                        attachment.Name ??= "Screenshot";
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

        private DateTimeOffset ParseEpochs(long epochMillis)
        {
            // According to the specification it should be milliseconds
            var date = DateTimeOffset.FromUnixTimeMilliseconds(epochMillis);

            // ..however, xunit seem to do seconds
            if(date < new DateTimeOffset(1990,1,1,0,0,0,TimeSpan.Zero))
            {
                date = DateTimeOffset.FromUnixTimeSeconds(epochMillis);
            }

            return date;
        }

        protected virtual TestSuiteRunDto GetOrCreateSuiteByName(string suiteIdentifier, Dictionary<string, TestSuiteRunDto> suites, CtrfReport<TResultsExtra, TEnvironmentExtra, TTestsExtra> report, TestRunDto run)
        {
            if(!suites.TryGetValue(suiteIdentifier, out var suite))
            {
                suite = new TestSuiteRunDto { Name = suiteIdentifier };

                suites[suiteIdentifier] = suite;
                run.Suites.Add(suite);
            }
            return suite;
        }

        protected virtual void ParseTestExtra(CtrfTest<TTestsExtra> test, TestCaseRunDto testDto)
        {
        }

        protected virtual CtrfTest<TTestsExtra> BuildTestExtra(TestSuiteRunDto suite, TestCaseRunDto testDto)
        {
            long duration = 0;
            var ctrfTest = new CtrfTest<TTestsExtra> { Duration = duration, Name = testDto.Name ?? "no-name", Status = CtrfResultMapping.GetCtrfStatusFromTestResult(testDto.Result ?? TestResult.Other) };
            ctrfTest.Suite = suite.ExternalId ?? suite.Name;

            if (testDto.Result is not null)
            {
                ctrfTest.RawStatus = testDto.Result.ToString();
            }
            if(testDto.Line is not null)
            {
                ctrfTest.Line = testDto.Line;
            }
            return ctrfTest;
        }

        protected virtual TResultsExtra BuildResultsExtra(TestRunDto run)
        {
            return new TResultsExtra();
        }

        public string Serialize(TestRunDto testRun)
        {
            AssignExternalIds(testRun);

            CtrfSummary summary = BuildSummary(testRun);

            List<CtrfTest<TTestsExtra>> tests = new();
            foreach (var suite in testRun.Suites)
            {
                foreach (var test in suite.Tests)
                {
                    var ctrfTest = BuildTestExtra(suite, test);
                    tests.Add(ctrfTest);
                }
            }
            var environment = new CtrfEnvironment<TEnvironmentExtra>();
            BuildEnvironment(testRun, environment);

            var resultsExtra = BuildResultsExtra(testRun);
            var tool = new CtrfTool() { Name = "TestBucket", Version = "1.0.0" };
            var results = new CtrfResults<TResultsExtra, TEnvironmentExtra, TTestsExtra>() { Summary = summary, Tests = tests.ToArray(), Tool = tool, Environment = environment, Extra = resultsExtra };
            var report = new CtrfReport<TResultsExtra, TEnvironmentExtra, TTestsExtra>() { Results = results, SpecVersion = "0.0.0", ReportFormat = "CTRF" };

            return JsonSerializer.Serialize(report);
        }

        private void AssignExternalIds(TestRunDto testRun)
        {
            testRun.ExternalId ??= Guid.NewGuid().ToString();
            foreach(var suite in testRun.Suites)
            {
                suite.ExternalId ??= Guid.NewGuid().ToString();
                foreach(var test in suite.Tests)
                {
                    test.ExternalId ??= Guid.NewGuid().ToString();
                }
            }
        }

        private static CtrfSummary BuildSummary(TestRunDto testRun)
        {
            var summary = new CtrfSummary()
            {
                Passed = testRun.Passed,
                Failed = testRun.Failed,
                Skipped = testRun.Skipped,
                Pending = 0,
                Other = testRun.Total - testRun.Passed - testRun.Passed - testRun.Skipped,
                Suites = testRun.Suites.Count,
            };
            if (testRun.StartedTime is not null)
            {
                summary.Start = testRun.StartedTime.Value.ToUnixTimeMilliseconds();
            }
            if (testRun.EndedTime is not null)
            {
                summary.Stop = testRun.EndedTime.Value.ToUnixTimeMilliseconds();
            }

            return summary;
        }

        private static void BuildEnvironment(TestRunDto testRun, CtrfEnvironment<TEnvironmentExtra> environment)
        {
            environment.ReportName = testRun.Name;

            if (testRun.Commit is not null)
            {
                environment.Commit = testRun.Commit;
            }
            else
            {
                foreach (TestCaseRunDto test in testRun.SelectTests(x => x.Commit is not null))
                {
                    environment.Commit = test.Commit;
                    break;
                }
            }
            if (testRun.SoftwareVersion is not null)
            {
                environment.AppVersion = testRun.SoftwareVersion;
            }
            else
            {
                foreach (TestCaseRunDto test in testRun.SelectTests(x => x.SoftwareVersion is not null))
                {
                    environment.AppVersion = test.SoftwareVersion;
                    break;
                }
            }
        }
    }
}
