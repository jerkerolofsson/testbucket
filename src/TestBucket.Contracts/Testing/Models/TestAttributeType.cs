using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Testing.Models;
public enum TestTraitType
{
    Custom,

    /// <summary>
    /// ID from original source
    /// </summary>
    ExternalId,
    Name,

    TestResult,

    // Failures

    FailureType,
    Message,
    CallStack,

    Module, // or assembly
    ClassName,
    Method,
    Line,

    Duration,

    TestedSoftwareVersion,
    TestedHardwareVersion,
    TestCategory,

    CreatedTime,
    StartedTime,
    EndedTime,

    /// <summary>
    /// stdout
    /// </summary>
    SystemOut,

    /// <summary>
    /// stderr
    /// </summary>
    SystemErr,

    Project,

    Version,
    Ci,
    Commit,
    Environment,
    Browser,
    Description,
    Tag,
    QualityCharacteristic,
    Area,
}
