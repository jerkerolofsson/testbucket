using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Traits.Core;

namespace TestBucket.Traits.MSTest;

/// <summary>
/// Security
/// Defines an attribute that will add create a trait related with ISO/IEC 25010 quality characteristic
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class SecurityTestAttribute : TestPropertyAttribute
{
    public SecurityTestAttribute() : base(TestTraitNames.QualityCharacteristic, "Security") { }
}

/// <summary>
/// Reliability
/// Defines an attribute that will add create a trait related with ISO/IEC 25010 quality characteristic
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class ReliabilityTestAttribute : TestPropertyAttribute
{
    public ReliabilityTestAttribute() : base(TestTraitNames.QualityCharacteristic, "Reliability") { }
}

/// <summary>
/// Performance Efficiency
/// Defines an attribute that will add create a trait related with ISO/IEC 25010 quality characteristic
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class PerformanceTestAttribute : TestPropertyAttribute
{
    public PerformanceTestAttribute() : base(TestTraitNames.QualityCharacteristic, "Performance Efficiency") { }
}

/// <summary>
/// Usability
/// Defines an attribute that will add create a trait related with ISO/IEC 25010 quality characteristic
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class UsabilityTestAttribute : TestPropertyAttribute
{
    public UsabilityTestAttribute() : base(TestTraitNames.QualityCharacteristic, "Usability") { }
}

/// <summary>
/// Functional Suitability
/// Defines an attribute that will add create a trait related with ISO/IEC 25010 quality characteristic
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class FunctionalTestAttribute : TestPropertyAttribute
{
    public FunctionalTestAttribute() : base(TestTraitNames.QualityCharacteristic, "Functional Suitability") { }
}