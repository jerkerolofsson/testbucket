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
    /// SUT platform name
    /// This could be a SoC or similar
    /// </summary>
    public const string SutPlatformName = nameof(SutPlatformName);

    /// <summary>
    /// SUT platform version
    /// </summary>
    public const string SutPlatformVersion = nameof(SutPlatformVersion);

    /// <summary>
    /// SUT OS (windows, android, ios, linux etc)
    /// </summary>
    public const string SutOperatingSystemName = nameof(SutOperatingSystemName);
    
    /// <summary>
    /// SUT OS Version
    /// </summary>
    public const string SutOperatingSystemVersion = nameof(SutOperatingSystemVersion);

    /// <summary>
    /// SUT Manufacturer
    /// </summary>
    public const string SutManufacturer = nameof(SutManufacturer);

    /// <summary>
    /// SUT Model
    /// </summary>
    public const string SutModel = nameof(SutModel);

    /// <summary>
    /// SW Version under test
    /// </summary>
    public const string SoftwareVersion = nameof(SoftwareVersion);

    /// <summary>
    /// SW build variant under test
    /// </summary>
    public const string SoftwareVariant = nameof(SoftwareVariant);

    /// <summary>
    /// HW Version under test
    /// </summary>
    public const string HardwareVersion = nameof(HardwareVersion);

    /// <summary>
    /// Longitude of the SUT (GNSS or other source)
    /// </summary>
    public const string SutLocationLongitude = nameof(SutLocationLongitude);

    /// <summary>
    /// Latitude of the SUT (GNSS or other source)
    /// </summary>
    public const string SutLocationLatitude = nameof(SutLocationLatitude);

    /// <summary>
    /// Branch / Head Ref.
    /// </summary>
    public const string Branch = nameof(Branch);

    /// <summary>
    /// Git commit hash
    /// </summary>
    public const string Commit = nameof(Commit);

}
