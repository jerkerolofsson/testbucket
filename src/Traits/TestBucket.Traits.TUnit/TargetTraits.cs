using TestBucket.Traits.Core;

namespace TestBucket.Traits.TUnit
{
    public class ComponentAttribute : PropertyAttribute
    {
        public ComponentAttribute(string componentUnderTestName) :
            base(TargetTraitNames.Component, componentUnderTestName)
        {
        }
    }

}
