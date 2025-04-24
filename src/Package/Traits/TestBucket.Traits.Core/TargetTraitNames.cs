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

    /// <summary>
    /// Product feature
    /// </summary>
    public const string Feature = nameof(Feature);

    // Runtime, not suitable for Attributes

    /// <summary>
    /// Project milestone
    /// </summary>
    public const string Milestone = nameof(Milestone);

    /// <summary>
    /// Project release
    /// </summary>
    public const string Release = nameof(Release);

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
