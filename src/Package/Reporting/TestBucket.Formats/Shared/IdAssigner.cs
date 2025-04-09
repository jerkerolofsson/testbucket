using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static System.Net.Mime.MediaTypeNames;

namespace TestBucket.Formats.Shared
{
    internal static class IdAssigner
    {
        /// <summary>
        /// Assigns ExternalId and InstanceId if not set
        /// </summary>
        /// <param name="testRun"></param>
        public static void AssignGuids(TestRunDto testRun)
        {
            testRun.ExternalId ??= Guid.NewGuid().ToString();
            if(testRun.Name is null)
            {
                testRun.Name = testRun.ExternalId;
            }
            testRun.InstanceId ??= Guid.NewGuid().ToString();

            foreach (var suite in testRun.Suites)
            {
                suite.ExternalId ??= Guid.NewGuid().ToString();
                suite.InstanceId ??= Guid.NewGuid().ToString();

                if (suite.Name is null)
                {
                    suite.Name = suite.ExternalId;
                }
                foreach (var test in suite.Tests)
                {
                    test.InstanceId ??= Guid.NewGuid().ToString();
                    test.ExternalId ??= Guid.NewGuid().ToString();
                    if (test.Name is null)
                    {
                        test.Name = test.ExternalId;
                    }
                }
            }
        }
    }
}
