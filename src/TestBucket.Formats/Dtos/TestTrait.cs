using TestBucket.Traits.Core;

namespace TestBucket.Formats.Dtos
{
    public class TestTrait : IComparable<TestTrait>
    {

        /// <summary>
        /// Well-known type of trait
        /// </summary>
        public TraitType Type { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        public TestTrait()
        {
            Type = TraitType.TestDescription;
            Value = "";
            Name = "Description";
        }
        public TestTrait(TraitType type, string value)
        {
            Name = type.ToString();
            Type = type;
            Value = value;
        }
        public TestTrait(TraitType type, string name, string value)
        {
            Name = name;
            Type = type;
            Value = value;
        }
        public TestTrait(string name, string value)
        {
            Name = name;
            Type = TraitType.Custom;
            Value = value;
        }

        public override string ToString()
        {
            return $"Name={Name}, Type={Type}, Value={Value}";
        }

        public int CompareTo(TestTrait? other)
        {
            if (other is null)
            {
                return -1;
            }

            return ToString().CompareTo(other.ToString());
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (obj is TestTrait attr)
            {
                return Name.Equals(attr.Name) && Type.Equals(attr.Type) && Value.Equals(attr.Value);
            }
            return false;
        }
    }

}
