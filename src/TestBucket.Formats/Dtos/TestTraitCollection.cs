using TestBucket.Traits.Core;

namespace TestBucket.Formats.Dtos
{
    public class TestTraitCollection
    {
        /// <summary>
        /// Traits / Properties
        /// </summary>
        public List<TestTrait> Traits { get; set; } = [];

        /// <summary>
        /// ID attribute
        /// </summary>
        public string? ExternalId
        {
            get => GetAttribute(TraitType.TestId);
            set => SetAttribute(TraitType.TestId, value);
        }

        /// <summary>
        /// Name attribute
        /// </summary>
        public string? Name
        {
            get => GetAttribute(TraitType.Name);
            set => SetAttribute(TraitType.Name, value);
        }
        /// <summary>
        /// Assembly attribute
        /// </summary>
        public string? Assembly
        {
            get => GetAttribute(TraitType.Assembly);
            set => SetAttribute(TraitType.Assembly, value);
        }

        /// <summary>
        /// Module attribute
        /// </summary>
        public string? Module
        {
            get => GetAttribute(TraitType.Module);
            set => SetAttribute(TraitType.Module, value);
        }

        /// <summary>
        /// Area attribute
        /// </summary>
        public string? Area
        {
            get => GetAttribute(TraitType.Area);
            set => SetAttribute(TraitType.Area, value);
        }


        /// <summary>
        /// TestedHardwareVersion attribute
        /// </summary>
        public string? HardwareVersion
        {
            get => GetAttribute(TraitType.HardwareVersion);
            set => SetAttribute(TraitType.HardwareVersion, value);
        }

        /// <summary>
        /// SystemOut attribute
        /// </summary>
        public string? SystemOut
        {
            get => GetAttribute(TraitType.SystemOut);
            set => SetAttribute(TraitType.SystemOut, value);
        }


        /// <summary>
        /// SystemErr attribute
        /// </summary>
        public string? SystemErr
        {
            get => GetAttribute(TraitType.SystemErr);
            set => SetAttribute(TraitType.SystemErr, value);
        }

        /// <summary>
        /// Version attribute
        /// </summary>
        public string? Version
        {
            get => GetAttribute(TraitType.Version);
            set => SetAttribute(TraitType.Version, value);
        }

        /// <summary>
        /// TestedSoftwareVersion attribute
        /// </summary>
        public string? SoftwareVersion
        {
            get => GetAttribute(TraitType.SoftwareVersion);
            set => SetAttribute(TraitType.SoftwareVersion, value);
        }
        /// <summary>
        /// Environment attribute
        /// </summary>
        public string? Environment
        {
            get => GetAttribute(TraitType.Environment);
            set => SetAttribute(TraitType.Environment, value);
        }

        /// <summary>
        /// TestCategory attribute
        /// </summary>
        public string? TestCategory
        {
            get => GetAttribute(TraitType.TestCategory);
            set => SetAttribute(TraitType.TestCategory, value);
        }

        /// <summary>
        /// Path/name of test
        /// </summary>
        public string? TestFilePath
        {
            get => GetAttribute(TraitType.TestFilePath);
            set => SetAttribute(TraitType.TestFilePath, value);
        }

        /// <summary>
        /// EndedTime attribute
        /// </summary>
        public DateTimeOffset? EndedTime
        {
            get => GetAttributeAsDateTimeOffset(TraitType.EndedTime);
            set => SetAttribute(TraitType.EndedTime, value);
        }

        /// <summary>
        /// StartedTime attribute
        /// </summary>
        public DateTimeOffset? StartedTime
        {
            get => GetAttributeAsDateTimeOffset(TraitType.StartedTime);
            set => SetAttribute(TraitType.StartedTime, value);
        }
        /// <summary>
        /// CreatedTime attribute
        /// </summary>
        public DateTimeOffset? CreatedTime
        {
            get => GetAttributeAsDateTimeOffset(TraitType.CreatedTime);
            set => SetAttribute(TraitType.CreatedTime, value);
        }

        /// <summary>
        /// Project attribute
        /// </summary>
        public string? Project
        {
            get => GetAttribute(TraitType.Project);
            set => SetAttribute(TraitType.Project, value);
        }

        /// <summary>
        /// TestId attribute
        /// </summary>
        public string? TestId
        {
            get => GetAttribute(TraitType.TestId);
            set => SetAttribute(TraitType.TestId, value);
        }

        /// <summary>
        /// Tag attribute
        /// </summary>
        public string? Tag
        {
            get => GetAttribute(TraitType.Tag);
            set => SetAttribute(TraitType.Tag, value);
        }

        /// <summary>
        /// Adds a test category
        /// </summary>
        /// <param name="value"></param>
        public void AddTestCategory(string value)
        {
            Traits.Add(new TestTrait(TraitType.TestCategory, value));
        }

        /// <summary>
        /// Adds a tag
        /// </summary>
        /// <param name="value"></param>
        public void AddTag(string value)
        {
            Traits.Add(new TestTrait(TraitType.Tag, value));
        }

        public void SetAttribute(TraitType type, int? value)
        {
            Traits.RemoveAll(x => x.Type == type);
            if (value is not null)
            {
                Traits.Add(new TestTrait(type, value.Value.ToString()));
            }
        }
        public void SetAttribute(TraitType type, DateTimeOffset? value)
        {
            Traits.RemoveAll(x => x.Type == type);
            if (value is not null)
            {
                Traits.Add(new TestTrait(type, value.Value.ToString("O")));
            }
        }
        public void SetAttribute(TraitType type, TimeSpan? value)
        {
            Traits.RemoveAll(x => x.Type == type);
            if (value is not null)
            {
                Traits.Add(new TestTrait(type, value.Value.ToString()));
            }
        }
        public void SetAttribute(TraitType type, string? value)
        {
            Traits.RemoveAll(x => x.Type == type);
            if (value is not null)
            {
                Traits.Add(new TestTrait(type, value));
            }
        }

        public string? GetAttribute(TraitType type)
        {
            foreach (var attr in Traits
                .Where(x => x.Type == type))
            {
                return attr.Value;
            }
            return default;
        }
        public int? GetAttributeAsInt32(TraitType type)
        {
            foreach (var attr in Traits
                .Where(x => x.Type == type))
            {
                if (int.TryParse(attr.Value, out var value))
                {
                    return value;
                }
            }
            return null;
        }
        public TimeSpan? GetAttributeAsTimeSpan(TraitType type)
        {
            foreach (var attr in Traits
                .Where(x => x.Type == type))
            {
                if (TimeSpan.TryParse(attr.Value, out var timeSpan))
                {
                    return timeSpan;
                }
            }
            return null;
        }
        public DateTimeOffset? GetAttributeAsDateTimeOffset(TraitType type)
        {
            foreach (var attr in Traits
                .Where(x => x.Type == type))
            {
                if (DateTimeOffset.TryParse(attr.Value, out var dateTime))
                {
                    return dateTime;
                }
            }
            return null;
        }
        public bool HasAttribute(TraitType type)
        {
            return Traits.Where(x => x.Type == type).Any();
        }
    }
}
