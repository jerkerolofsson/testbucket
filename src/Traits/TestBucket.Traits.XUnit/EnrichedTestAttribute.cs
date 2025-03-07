using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Xunit;


namespace TestBucket.Traits.Xunit
{
    public class EnrichedTestAttribute : BeforeAfterTestAttribute
    {
        public EnrichedTestAttribute()
        {
        }

        public override void After(MethodInfo methodUnderTest, IXunitTest test)
        {
            if(TestContext.Current.TestState?.Result == TestResult.Failed)
            { 
            }
            //TestContext.Current.AddAttachment(Traits.Core.TestTraitNames.MachineName, Encoding.UTF8.GetBytes(Environment.MachineName), "text/plain");
            
            TestContext.Current.AddAttachment(Traits.Core.TestTraitNames.MachineName, Environment.MachineName);

            var className = test.TestMethod.TestClass.Class.Name;
            var namespaceName = test.TestMethod.TestClass.Class.Namespace;

            TestContext.Current.AddAttachment(AutomationTraitNames.ClassName, $"{namespaceName}.{className}");
            TestContext.Current.AddAttachment(AutomationTraitNames.MethodName, test.TestMethod.MethodName);

            var assemblyName = test.TestMethod.TestClass.Class.Assembly.GetName().Name;
            if (assemblyName is not null)
            {
                TestContext.Current.AddAttachment(AutomationTraitNames.Assembly, assemblyName);
            }
        }
    }
}
