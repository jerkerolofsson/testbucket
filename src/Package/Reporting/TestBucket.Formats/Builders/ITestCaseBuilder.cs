using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Traits.Core;

namespace TestBucket.Formats.Builders
{
    public interface ITestCaseBuilder : ISharedBuilderBase
    {
        /// <summary>
        /// Adds an attachment
        /// </summary>
        /// <param name="name"></param>
        /// <param name="mediaType"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        ITestCaseBuilder AddAttachment(string name, string mediaType, byte[] data);

        /// <summary>
        /// Adds a test case
        /// </summary>
        /// <returns></returns>
        ITestCaseBuilder AddTestCase();

        /// <summary>
        /// Adds a known trait (or property)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ITestCaseBuilder AddTrait(TraitType type, string value);

        /// <summary>
        /// Adds a custom trait (or property)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ITestCaseBuilder AddTrait(string name, string value);

        /// <summary>
        /// Sets the name of the test case
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ITestCaseBuilder SetName(string name);

        /// <summary>
        /// Sets the test result
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public ITestCaseBuilder SetResult(TestResult result, string? message = null);

    }
}
