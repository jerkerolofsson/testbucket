using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Fields;
public enum FieldDataSourceType
{
    /// <summary>
    /// User defined list (default)
    /// </summary>
    List = 0,

    /// <summary>
    /// HTTP URL or similar
    /// </summary>
    External = 1,

    /// <summary>
    /// Milestones
    /// </summary>
    Milestones = 2,

    /// <summary>
    /// Releases
    /// </summary>
    Releases = 3,

    /// <summary>
    /// Features
    /// </summary>
    Features = 4,

    /// <summary>
    /// Components
    /// </summary>
    Components = 5,

    /// <summary>
    /// System
    /// </summary>
    Systems = 6,

    /// <summary>
    /// Layers
    /// </summary>
    Layers = 7,

    /// <summary>
    /// Commit
    /// </summary>
    Commit = 8,

    /// <summary>
    /// Issue
    /// </summary>
    Issue = 9,

    /// <summary>
    /// Requirement
    /// </summary>
    Requirement = 10,

    /// <summary>
    /// TestCase
    /// </summary>
    TestCase = 11,



    // Enums
    Dock = 12,

    /// <summary>
    /// Labels
    /// </summary>
    Labels = 13,
}
