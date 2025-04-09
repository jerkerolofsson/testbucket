using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Formats.JUnit;
public class JUnitSerializerOptions
{
    /// <summary>
    /// Extracts name and external ID properties from the test suite naming convention used
    /// by xunit: Test collection for TestBucket.Formats.UnitTests.XUnit.XUnitSerializerTests (id: da8cd3d88ee6e9d4988dcd095d84dc5287c1fa9c88564f2d34192c011f8ae07e)
    /// </summary>
    public bool ProcessXunitCollectionName { get; set; } = true;

    /// <summary>
    /// Creates test case folders from the nested test suites
    /// </summary>
    public bool CreateFoldersFromNestedTestSuites { get; set; } = true;
}
