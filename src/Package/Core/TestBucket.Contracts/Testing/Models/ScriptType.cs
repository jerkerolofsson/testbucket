using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Testing.Models;

/// <summary>
/// Defines the type of implementation of a test case.
/// 
/// The type defines the UI of the test
/// </summary>
public enum ScriptType
{
    /// <summary>
    /// A scripted tests that contains a single field that describes the test actions, including expected results.
    /// 
    /// Test result not tracked on individual steps, but only on the test case level.
    /// </summary>
    ScriptedDefault = 0,

    /// <summary>
    /// A scripted test that contains multiple well defined steps, where each step has a separate field for 
    /// - action
    /// - expected result
    /// 
    /// These tests allow tracking test results on a step level.
    /// </summary>
    ScriptedSteps = 1,
}
