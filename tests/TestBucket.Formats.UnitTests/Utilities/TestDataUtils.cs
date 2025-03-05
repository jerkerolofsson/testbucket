using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Formats.UnitTests.Utilities
{
    internal class TestDataUtils
    {
        public static string GetResourceXml(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream(resourceName);
            if(stream is null)
            {
                var resourceNames = string.Join(", ", assembly.GetManifestResourceNames());
                throw new InvalidDataException($"The resource '{resourceName}' was not found in assembly: '{assembly.GetName()}'\nAvailable resources: {resourceNames}");
            }
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
