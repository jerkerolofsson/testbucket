using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Traits.Core;

namespace TestBucket.Formats.Builders
{
    /// <summary>
    /// Helper class to build and serialize result files
    /// </summary>
    public class TestResultFileBuilder : ITestRunBuilder
    {
        private readonly TestRunDto _testRunDto = new TestRunDto();

        /// <summary>
        /// Sets the name of the test run
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ITestRunBuilder SetName(string name)
        {
            _testRunDto.Name = name;
            return this;
        }

        public ITestRunBuilder SetTestExecutionDateRange(DateTimeOffset started, DateTimeOffset ended)
        {
            _testRunDto.StartedTime = started;
            _testRunDto.EndedTime = ended;
            return this;
        }

        /// <summary>
        /// Returns a string containing the serialized test
        /// </summary>
        /// <returns></returns>
        public string Build(TestResultFormat format)
        {
            ITestResultSerializer serializer = TestResultSerializerFactory.Create(format);
            return serializer.Serialize(_testRunDto);
        }

        public ITestSuiteBuilder AddTestSuite()
        {
            var suite = new TestSuiteRunDto();
            _testRunDto.Suites.Add(suite);

            return new TestSuiteBuilder(suite, this);
        }

        internal class SharedBuilderBase(TestResultFileBuilder root) : ISharedBuilderBase
        {
            public ITestSuiteBuilder AddTestSuite() => root.AddTestSuite();
            public string Build(TestResultFormat format) => root.Build(format);
        }

        internal class TestCaseBuilder : SharedBuilderBase, ITestCaseBuilder
        {
            private readonly TestCaseRunDto _test;
            private readonly ITestSuiteBuilder _suiteBuilder;

            public TestCaseBuilder(TestCaseRunDto test, ITestSuiteBuilder suiteBuilder, TestResultFileBuilder root) : base(root)
            {
                _test = test;
                _suiteBuilder = suiteBuilder;
            }

            public ITestCaseBuilder AddAttachment(string name, string mediaType, byte[] data)
            {
                var attachment = new AttachmentDto
                {
                    ContentType = mediaType,
                    Data = data,
                    Name = name
                };
                _test.Attachments.Add(attachment);
                return this;
            }

            public ITestCaseBuilder AddTrait(TraitType traitType, string value)
            {
                _test.Traits.Add(new TestTrait { Name = traitType.ToString(), Type = traitType, Value = value });
                return this;
            }

            public ITestCaseBuilder AddTrait(string name, string value)
            {
                _test.Traits.Add(new TestTrait { Name = name, Value = value, Type = TraitType.Custom });
                return this;
            }

            public ITestCaseBuilder SetName(string name)
            {
                _test.Name = name;
                return this;
            }

            public ITestCaseBuilder AddTestCase() => _suiteBuilder.AddTestCase();

            public ITestCaseBuilder SetResult(TestResult result, string? message = null)
            {
                _test.Result = result;  
                _test.Message = message;
                return this;
            }
        }
        internal class TestSuiteBuilder : SharedBuilderBase, ITestSuiteBuilder
        {
            private readonly TestSuiteRunDto _suite;
            private readonly TestResultFileBuilder _root;

            public TestSuiteBuilder(TestSuiteRunDto suite, TestResultFileBuilder root) : base(root)
            {
                _suite = suite;
                _root = root;
            }

            public ITestSuiteBuilder SetName(string name)
            {
                _suite.Name = name;
                return this;
            }

            public ITestCaseBuilder AddTestCase()
            {
                var test = new TestCaseRunDto();
                _suite.Tests.Add(test);
                return new TestCaseBuilder(test, this, _root);
            }
        }
    }
}
