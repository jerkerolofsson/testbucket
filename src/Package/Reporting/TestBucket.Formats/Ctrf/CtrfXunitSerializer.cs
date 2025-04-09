using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using TestBucket.Formats.Ctrf.Models;
using TestBucket.Formats.Shared;
using TestBucket.Traits.Core;

namespace TestBucket.Formats.Ctrf
{
    public class CtrfXunitSerializer : CtrfSerializer<XunitResultsExtra, XunitEnvironmentExtra, XunitTestsExtra>
    {
        /// <summary>
        /// Looks up the suite from the results extra object containing suites by ID
        /// </summary>
        /// <param name="suiteIdentifier"></param>
        /// <param name="suites"></param>
        /// <param name="report"></param>
        /// <param name="run"></param>
        /// <returns></returns>
        protected override TestSuiteRunDto GetOrCreateSuiteByName(string suiteIdentifier, Dictionary<string, TestSuiteRunDto> suites, CtrfReport<XunitResultsExtra, XunitEnvironmentExtra, XunitTestsExtra> report, TestRunDto run)
        {
            if(suites.TryGetValue(suiteIdentifier, out var suite))
            {
                return suite;
            }

            if(report.Results.Extra?.Suites is not null)
            {
                var ctrfSuite = report.Results.Extra.Suites.Where(x => x.Id == suiteIdentifier).FirstOrDefault();
                if (ctrfSuite is not null)
                {
                    suite = new TestSuiteRunDto
                    {
                        Name = ctrfSuite.Id,
                        ExternalId = ctrfSuite.Id,
                        Environment = ctrfSuite.Environment,
                        TestFilePath = ctrfSuite.FilePath,
                        StartedTime = DateTimeOffset.FromUnixTimeMilliseconds(ctrfSuite.Start),
                        EndedTime = DateTimeOffset.FromUnixTimeMilliseconds(ctrfSuite.Stop),
                    };
                    suites[suiteIdentifier] = suite;
                    run.Suites.Add(suite);

                    return suite;
                }
            }

            return base.GetOrCreateSuiteByName(suiteIdentifier, suites, report, run);
        }

        protected override XunitResultsExtra BuildResultsExtra(TestRunDto run)
        {
            var extra = base.BuildResultsExtra(run);

            var suites = new List<XunitCtrfSuite>();
            foreach(var suite in run.Suites)
            {
                var ctrfSuite = new XunitCtrfSuite();
                suites.Add(ctrfSuite);

                ctrfSuite.Id = suite.ExternalId;
                ctrfSuite.Environment = suite.Environment;
                ctrfSuite.FilePath = suite.TestFilePath;
                ctrfSuite.Duration = (int)suite.Duration.TotalMilliseconds;
                if(suite.StartedTime is not null)
                {
                    ctrfSuite.Start = suite.StartedTime.Value.ToUnixTimeMilliseconds();
                }
                if (suite.EndedTime is not null)
                {
                    ctrfSuite.Stop = suite.EndedTime.Value.ToUnixTimeMilliseconds();
                }
            }

            extra.Suites = suites.ToArray();

            return extra;
        }

        protected override CtrfTest<XunitTestsExtra> BuildTestExtra(TestSuiteRunDto suite,  TestCaseRunDto testDto)
        {
            var ctrfTest = base.BuildTestExtra(suite, testDto);

            ctrfTest.Extra = new XunitTestsExtra()
            {
                Traits = new(),
                Attachments = new(),
                Id = testDto.ExternalId,
                Collection = suite.ExternalId,
                Method = testDto.Method,
                Type = testDto.ClassName
            };

            foreach (var trait in testDto.Traits.DistinctBy(x => x.Name))
            {
                if(_nativeAttributes.Contains(trait.Type))
                {
                    continue;
                }

                string[] values = testDto.Traits.Where(x => x.Name == trait.Name).Select(x => x.Value).ToArray();
                ctrfTest.Extra.Traits.Add(trait.Name, values);
            }

            foreach(var attachment in testDto.Attachments)
            {
                ctrfTest.Extra.Attachments.Attachments.Add(attachment); 
            }

            return ctrfTest;
        }
      
        protected override void ParseTestExtra(CtrfTest<XunitTestsExtra> test, TestCaseRunDto testDto)
        {
            if(test.Extra is not null)
            {
                if(test.Extra.Traits is not null)
                {
                    foreach(var trait in test.Extra.Traits)
                    {
                        var name = trait.Key;
                        var values = trait.Value;
                        foreach(var value in values)
                        {
                            var traitType = TestTraitHelper.GetTestTraitType(name);
                            testDto.Traits.Add(new TestTrait(traitType, name, value.ToString()));
                        }
                    }
                }
                if(test.Extra.Attachments is not null)
                {
                    foreach (var attachmentTrait in test.Extra.Attachments.Traits)
                    {
                        var traitType = TestTraitHelper.GetTestTraitType(attachmentTrait.Name);
                        testDto.Traits.Add(new TestTrait(traitType, attachmentTrait.Name, attachmentTrait.Value));
                    }
                    foreach (var attachment in test.Extra.Attachments.Attachments)
                    {
                       testDto.Attachments.Add(attachment);
                    }
                }
            }
        }
    }
}
