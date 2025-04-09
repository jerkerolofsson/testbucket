using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Formats.Builders
{
    public interface ITestRunBuilder
    {
        /// <summary>
        /// Builds the result file, returning the content as a string (XML, json etc depending on format)
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>

        string Build(TestResultFormat format);

        /// <summary>
        /// Sets the name of the run
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ITestRunBuilder SetName(string name);

        /// <summary>
        /// Adds a test suite
        /// </summary>
        /// <returns></returns>
        public ITestSuiteBuilder AddTestSuite();
        ITestRunBuilder SetTestExecutionDateRange(DateTimeOffset started, DateTimeOffset ended);
    }
}
