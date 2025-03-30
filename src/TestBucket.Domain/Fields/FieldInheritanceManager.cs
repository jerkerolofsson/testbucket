using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Fields
{
    public class FieldInheritanceManager
    {
        /// <summary>
        /// Assigns inherited fields that are applicable for a test case in the following order:
        /// 1. Team
        /// 2. Project
        /// 3. TestSuite
        /// 4. TestSuiteFolder (root)
        ///  ...
        /// n. TestSuiteFolder (parent)
        /// </summary>
        /// <param name="testCase"></param>
        /// <returns></returns>
        public Task AssignInheritedFieldsAsync(TestCase testCase)
        {
            throw new NotImplementedException("todo");
        }
    }
}
