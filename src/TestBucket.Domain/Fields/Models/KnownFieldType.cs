namespace TestBucket.Domain.Fields.Models;

/// <summary>
/// For reporting reasons, some fields are linked to a known field
/// </summary>
public enum KnownFieldType
{
    Custom = 1,

    Priority,
    SoftwareVersion,
    HardwareVersion,
    QualityCharacteristic,

    /// <summary>
    /// A sub component in the product, like a vertical slice, or a tier, or a sub-system
    /// </summary>
    ProductComponent,

    /// <summary>
    /// E2E, Integration, Unit
    /// </summary>
    TestType,

    /// <summary>
    /// Manual / automated
    /// </summary>
    TestExecutionType,

    /// <summary>
    /// Test method (White box, Black box..)
    /// Can be used for coverage analysis
    /// </summary>
    TestMethod,
}
