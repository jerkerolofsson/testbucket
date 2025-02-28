
namespace TestBucket.Domain.Testing.Formats.Dtos
{
    public class TestTrait : IComparable<TestTrait>
    {
        public TestTraitType Type { get; set; }
        public string Value { get; set; }
        public string Name { get; set; }

        public TestTrait()
        {
            Type = TestTraitType.Description;
            Value = "";
            Name = "Description";
        }
        public TestTrait(TestTraitType type, string value)
        {
            Name = type.ToString();
            Type = type;
            Value = value;
        }
        public TestTrait(TestTraitType type, string name, string value)
        {
            Name = name;
            Type = type;
            Value = value;
        }
        public TestTrait(string name, string value)
        {
            Name = name;
            Type = TestTraitType.Custom;
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
