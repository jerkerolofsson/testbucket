using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Testing.Models;

/// <summary>
/// Defines rules for an asset (account, resource) which is required to execute the test case
/// </summary>
public class TestCaseDependency
{
    /// <summary>
    /// The test case requires a resource of this specified type
    /// </summary>
    public string? ResourceType { get; set; }
}
