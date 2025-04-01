namespace TestBucket.Formats.Builders
{
    public interface ITestSuiteBuilder : ISharedBuilderBase
    {
        /// <summary>
        /// Sets the name of the suite
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ITestSuiteBuilder SetName(string name);

        /// <summary>
        /// Adds a test case
        /// </summary>
        /// <returns></returns>
        ITestCaseBuilder AddTestCase();
    }
}
