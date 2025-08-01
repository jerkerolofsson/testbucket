using TestBucket.Traits.Core;

namespace TestBucket.Traits.MSTest;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class CoveredRequirementAttribute : TestPropertyAttribute
{
    public CoveredRequirementAttribute(string requirementId) : base(TestTraitNames.CoveredRequirement, requirementId)
    {
    }
}
