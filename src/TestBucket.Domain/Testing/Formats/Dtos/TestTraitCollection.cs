
namespace TestBucket.Domain.Testing.Formats.Dtos
{
    public class TestTraitCollection
    {
        public List<TestTrait> Attributes { get; set; } = new();

        /// <summary>
        /// ID attribute
        /// </summary>
        public string? ExternalId
        {
            get => GetAttribute(TestTraitType.ExternalId);
            set => SetAttribute(TestTraitType.ExternalId, value);
        }

        /// <summary>
        /// Name attribute
        /// </summary>
        public string? Name
        {
            get => GetAttribute(TestTraitType.Name);
            set => SetAttribute(TestTraitType.Name, value);
        }

        /// <summary>
        /// Module/Assembly attribute
        /// </summary>
        public string? Module
        {
            get => GetAttribute(TestTraitType.Module);
            set => SetAttribute(TestTraitType.Module, value);
        }

        /// <summary>
        /// Area attribute
        /// </summary>
        public string? Area
        {
            get => GetAttribute(TestTraitType.Area);
            set => SetAttribute(TestTraitType.Area, value);
        }


        /// <summary>
        /// TestedHardwareVersion attribute
        /// </summary>
        public string? HardwareVersion
        {
            get => GetAttribute(TestTraitType.TestedHardwareVersion);
            set => SetAttribute(TestTraitType.TestedHardwareVersion, value);
        }

        /// <summary>
        /// SystemOut attribute
        /// </summary>
        public string? SystemOut
        {
            get => GetAttribute(TestTraitType.SystemOut);
            set => SetAttribute(TestTraitType.SystemOut, value);
        }


        /// <summary>
        /// SystemErr attribute
        /// </summary>
        public string? SystemErr
        {
            get => GetAttribute(TestTraitType.SystemErr);
            set => SetAttribute(TestTraitType.SystemErr, value);
        }

        /// <summary>
        /// Version attribute
        /// </summary>
        public string? Version
        {
            get => GetAttribute(TestTraitType.Version);
            set => SetAttribute(TestTraitType.Version, value);
        }

        /// <summary>
        /// TestedSoftwareVersion attribute
        /// </summary>
        public string? SoftwareVersion
        {
            get => GetAttribute(TestTraitType.TestedSoftwareVersion);
            set => SetAttribute(TestTraitType.TestedSoftwareVersion, value);
        }
        /// <summary>
        /// Environment attribute
        /// </summary>
        public string? Environment
        {
            get => GetAttribute(TestTraitType.Environment);
            set => SetAttribute(TestTraitType.Environment, value);
        }

        /// <summary>
        /// TestCategory attribute
        /// </summary>
        public string? TestCategory
        {
            get => GetAttribute(TestTraitType.TestCategory);
            set => SetAttribute(TestTraitType.TestCategory, value);
        }

        /// <summary>
        /// EndedTime attribute
        /// </summary>
        public DateTimeOffset? EndedTime
        {
            get => GetAttributeAsDateTimeOffset(TestTraitType.EndedTime);
            set => SetAttribute(TestTraitType.EndedTime, value);
        }

        /// <summary>
        /// StartedTime attribute
        /// </summary>
        public DateTimeOffset? StartedTime
        {
            get => GetAttributeAsDateTimeOffset(TestTraitType.StartedTime);
            set => SetAttribute(TestTraitType.StartedTime, value);
        }
        /// <summary>
        /// CreatedTime attribute
        /// </summary>
        public DateTimeOffset? CreatedTime
        {
            get => GetAttributeAsDateTimeOffset(TestTraitType.CreatedTime);
            set => SetAttribute(TestTraitType.CreatedTime, value);
        }

        /// <summary>
        /// Project attribute
        /// </summary>
        public string? Project
        {
            get => GetAttribute(TestTraitType.Project);
            set => SetAttribute(TestTraitType.Project, value);
        }
        /// <summary>
        /// Tag attribute
        /// </summary>
        public string? Tag
        {
            get => GetAttribute(TestTraitType.Tag);
            set => SetAttribute(TestTraitType.Tag, value);
        }


        /// <summary>
        /// Adds a test category
        /// </summary>
        /// <param name="value"></param>
        public void AddTestCategory(string value)
        {
            Attributes.Add(new TestTrait(TestTraitType.TestCategory, value));
        }

        /// <summary>
        /// Adds a tag
        /// </summary>
        /// <param name="value"></param>
        public void AddTag(string value)
        {
            Attributes.Add(new TestTrait(TestTraitType.Tag, value));
        }

        public void SetAttribute(TestTraitType type, int? value)
        {
            Attributes.RemoveAll(x => x.Type == type);
            if (value is not null)
            {
                Attributes.Add(new TestTrait(type, value.Value.ToString()));
            }
        }
        public void SetAttribute(TestTraitType type, DateTimeOffset? value)
        {
            Attributes.RemoveAll(x => x.Type == type);
            if (value is not null)
            {
                Attributes.Add(new TestTrait(type, value.Value.ToString("O")));
            }
        }
        public void SetAttribute(TestTraitType type, TimeSpan? value)
        {
            Attributes.RemoveAll(x => x.Type == type);
            if (value is not null)
            {
                Attributes.Add(new TestTrait(type, value.Value.ToString()));
            }
        }
        public void SetAttribute(TestTraitType type, string? value)
        {
            Attributes.RemoveAll(x => x.Type == type);
            if (value is not null)
            {
                Attributes.Add(new TestTrait(type, value));
            }
        }

        public string? GetAttribute(TestTraitType type)
        {
            foreach (var attr in Attributes
                .Where(x => x.Type == type))
            {
                return attr.Value;
            }
            return default;
        }
        public int? GetAttributeAsInt32(TestTraitType type)
        {
            foreach (var attr in Attributes
                .Where(x => x.Type == type))
            {
                if (int.TryParse(attr.Value, out var value))
                {
                    return value;
                }
            }
            return null;
        }
        public TimeSpan? GetAttributeAsTimeSpan(TestTraitType type)
        {
            foreach (var attr in Attributes
                .Where(x => x.Type == type))
            {
                if (TimeSpan.TryParse(attr.Value, out var timeSpan))
                {
                    return timeSpan;
                }
            }
            return null;
        }
        public DateTimeOffset? GetAttributeAsDateTimeOffset(TestTraitType type)
        {
            foreach (var attr in Attributes
                .Where(x => x.Type == type))
            {
                if (DateTimeOffset.TryParse(attr.Value, out var dateTime))
                {
                    return dateTime;
                }
            }
            return null;
        }
        public bool HasAttribute(TestTraitType type)
        {
            return Attributes.Where(x => x.Type == type).Any();
        }
    }
}
