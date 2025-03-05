namespace TestBucket.Traits.Core;

/// <summary>
/// Traits that relate to the device/system/application under test
/// </summary>
public class TargetTraitNames
{
    /// <summary>
    /// Component under test
    /// </summary>
    public const string Component = nameof(Component);

    // Runtime, not suitable for Attributes

    /// <summary>
    /// SW Version under test
    /// </summary>
    public const string SoftwareVersion = nameof(SoftwareVersion);

    /// <summary>
    /// HW Version under test
    /// </summary>
    public const string HardwareVersion = nameof(HardwareVersion);

    /// <summary>
    /// Git commit hash
    /// </summary>
    public const string Commit = nameof(Commit);

}
