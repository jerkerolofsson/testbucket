namespace TestBucket.Traits.Xunit;

/// <summary>
/// ReliaSecuritybility
/// Defines an attribute that will add create a trait related with ISO/IEC 25010 quality characteristic
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class SecurityTestAttribute : TraitAttachmentPropertyAttribute
{
    public SecurityTestAttribute() : base(TestTraitNames.QualityCharacteristic, "Security") { }
}

/// <summary>
/// Reliability
/// Defines an attribute that will add create a trait related with ISO/IEC 25010 quality characteristic
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class ReliabilityTestAttribute : TraitAttachmentPropertyAttribute
{
    public ReliabilityTestAttribute() : base(TestTraitNames.QualityCharacteristic, "Reliability") { }
}

/// <summary>
/// Performance Efficiency
/// Defines an attribute that will add create a trait related with ISO/IEC 25010 quality characteristic
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class PerformanceTestAttribute : TraitAttachmentPropertyAttribute
{
    public PerformanceTestAttribute() : base(TestTraitNames.QualityCharacteristic, "Performance Efficiency") { }
}

/// <summary>
/// Functional Suitability
/// Defines an attribute that will add create a trait related with ISO/IEC 25010 quality characteristic
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class FunctionalTestAttribute : TraitAttachmentPropertyAttribute
{
    public FunctionalTestAttribute() : base(TestTraitNames.QualityCharacteristic, "Functional Suitability"){}
}

/// <summary>
/// Defines an attribute that will add create a Tag trait
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class TagAttribute : CustomTraitAttribute
{
    public TagAttribute(string tagName) :
        base(TestTraitNames.Tag, tagName)
    {
    }
}

/// <summary>
/// Defines an attribute that will add create a Description trait
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class TestDescriptionAttribute : TraitAttachmentPropertyAttribute
{
    public TestDescriptionAttribute(string description) : 
        base(TestTraitNames.TestDescription, description)
    {
    }
}

/// <summary>
/// Defines an attribute that will define a Test ID trait
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class TestIdAttribute : TraitAttachmentPropertyAttribute
{
    public TestIdAttribute(string testId) :
        base(TestTraitNames.TestId, testId)
    {
    }
}

/// <summary>
/// Defines an attribute that will link a requirement
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class CoveredRequirementAttribute : TraitAttachmentPropertyAttribute
{
    public CoveredRequirementAttribute(string description) :
        base(TestTraitNames.CoveredRequirement, description)
    {
    }
}

/// <summary>
/// Defines an attribute that will link an issue
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class CoveredIssueAttribute : TraitAttachmentPropertyAttribute
{
    public CoveredIssueAttribute(string description) :
        base(TestTraitNames.CoveredIssue, description)
    {
    }
}