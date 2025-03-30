using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Formats;
public class ImportHandlingOptions
{
    /// <summary>
    /// Creates folders from the class package/namespace:
    /// 
    /// TestBucket.Formats => ["TestBucket", "Formats]
    /// 
    /// This option overrides settings for implementation specific handling
    /// </summary>
    public bool CreateFoldersFromClassNamespace { get; set; } = true;

    /// <summary>
    /// If the test name starts with the class name, it is removed from the name
    /// </summary>
    public bool RemoveClassNameFromTestName { get; set; } = true;

    /// <summary>
    /// If an assembly name is present, use this to create the test suite overriding any 
    /// values in the import settings
    /// </summary>
    public bool CreateTestSuiteFromAssemblyName { get; set; } = true;

    /// <summary>
    /// Special options for junit
    /// </summary>
    public JUnit.JUnitSerializerOptions Junit { get; set; } = new();
    
    /// <summary>
    /// Test run, if null: a new one will be created
    /// </summary>
    public long? TestRunId { get; set; }

    /// <summary>
    /// If set, the results will be imported and linked to a single test case
    /// </summary>
    public long? TestCaseId { get; set; }
}
