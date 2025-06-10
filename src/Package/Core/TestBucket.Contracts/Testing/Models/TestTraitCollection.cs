using System.Text.Json.Serialization;

using TestBucket.Traits.Core;

namespace TestBucket.Formats.Dtos
{
    /// <summary>
    /// Collection of traits/properties/attributes
    /// </summary>
    public class TestTraitCollection
    {
        /// <summary>
        /// Traits / Properties
        /// </summary>
        public List<TestTrait> Traits { get; set; } = [];

        #region Convenience trait access properties

        /// <summary>
        /// ID attribute
        /// 
        /// For TestCaseRun this is the ID of a test case
        /// 
        /// </summary>
        [JsonIgnore]
        public string? ExternalId
        {
            get => GetAttribute(TraitType.TestId);
            set => SetAttribute(TraitType.TestId, value);
        }

        /// <summary>
        /// ID of a specific run
        /// </summary>
        [JsonIgnore]
        public string? InstanceId
        {
            get => GetAttribute(TraitType.InstanceId);
            set => SetAttribute(TraitType.InstanceId, value);
        }


        /// <summary>
        /// Milestone
        /// </summary>
        [JsonIgnore]
        public string? Milestone
        {
            get => GetAttribute(TraitType.Milestone);
            set => SetAttribute(TraitType.Milestone, value);
        }

        /// <summary>
        /// Name attribute (test case name, test suite name, test run name etc)
        /// </summary>
        [JsonIgnore]
        public string? Name
        {
            get => GetAttribute(TraitType.Name);
            set => SetAttribute(TraitType.Name, value);
        }

        /// <summary>
        /// Username of the user running the test
        /// </summary>
        [JsonIgnore]
        public string? InstanceUserName
        {
            get => GetAttribute(TraitType.InstanceUserName);
            set => SetAttribute(TraitType.InstanceUserName, value);
        }

        /// <summary>
        /// Assembly attribute
        /// </summary>
        [JsonIgnore]
        public string? Assembly
        {
            get => GetAttribute(TraitType.Assembly);
            set => SetAttribute(TraitType.Assembly, value);
        }

        /// <summary>
        /// Module attribute
        /// </summary>
        [JsonIgnore]
        public string? Module
        {
            get => GetAttribute(TraitType.Module);
            set => SetAttribute(TraitType.Module, value);
        }

        /// <summary>
        /// Area attribute
        /// </summary>
        [JsonIgnore]
        public string? Area
        {
            get => GetAttribute(TraitType.Area);
            set => SetAttribute(TraitType.Area, value);
        }


        /// <summary>
        /// TestedHardwareVersion attribute
        /// </summary>
        [JsonIgnore]
        public string? HardwareVersion
        {
            get => GetAttribute(TraitType.HardwareVersion);
            set => SetAttribute(TraitType.HardwareVersion, value);
        }

        /// <summary>
        /// SystemOut attribute
        /// </summary>
        [JsonIgnore]
        public string? SystemOut
        {
            get => GetAttribute(TraitType.SystemOut);
            set => SetAttribute(TraitType.SystemOut, value);
        }


        /// <summary>
        /// SystemErr attribute
        /// </summary>
        [JsonIgnore]
        public string? SystemErr
        {
            get => GetAttribute(TraitType.SystemErr);
            set => SetAttribute(TraitType.SystemErr, value);
        }

        /// <summary>
        /// Version attribute
        /// </summary>
        [JsonIgnore]
        public string? Version
        {
            get => GetAttribute(TraitType.Version);
            set => SetAttribute(TraitType.Version, value);
        }

        /// <summary>
        /// Commit attribute
        /// </summary>
        public string? Commit
        {
            get => GetAttribute(TraitType.Commit);
            set => SetAttribute(TraitType.Commit, value);
        }


        /// <summary>
        /// TestedSoftwareVersion attribute
        /// </summary>
        [JsonIgnore]
        public string? SoftwareVersion
        {
            get => GetAttribute(TraitType.SoftwareVersion);
            set => SetAttribute(TraitType.SoftwareVersion, value);
        }

        /// <summary>
        /// Environment attribute
        /// </summary>
        [JsonIgnore]
        public string? Environment
        {
            get => GetAttribute(TraitType.Environment);
            set => SetAttribute(TraitType.Environment, value);
        }

        /// <summary>
        /// TestCategory attribute
        /// </summary>
        [JsonIgnore]
        public string? TestCategory
        {
            get => GetAttribute(TraitType.TestCategory);
            set => SetAttribute(TraitType.TestCategory, value);
        }

        /// <summary>
        /// Path/name of test
        /// </summary>
        [JsonIgnore]
        public string? TestFilePath
        {
            get => GetAttribute(TraitType.TestFilePath);
            set => SetAttribute(TraitType.TestFilePath, value);
        }

        /// <summary>
        /// EndedTime attribute
        /// </summary>
        [JsonIgnore]
        public DateTimeOffset? EndedTime
        {
            get => GetAttributeAsDateTimeOffset(TraitType.EndedTime);
            set => SetAttribute(TraitType.EndedTime, value);
        }

        /// <summary>
        /// StartedTime attribute
        /// </summary>
        [JsonIgnore]
        public DateTimeOffset? StartedTime
        {
            get => GetAttributeAsDateTimeOffset(TraitType.StartedTime);
            set => SetAttribute(TraitType.StartedTime, value);
        }

        /// <summary>
        /// CreatedTime attribute
        /// </summary>
        [JsonIgnore]
        public DateTimeOffset? CreatedTime
        {
            get => GetAttributeAsDateTimeOffset(TraitType.CreatedTime);
            set => SetAttribute(TraitType.CreatedTime, value);
        }

        /// <summary>
        /// Project attribute
        /// </summary>
        [JsonIgnore]
        public string? Project
        {
            get => GetAttribute(TraitType.Project);
            set => SetAttribute(TraitType.Project, value);
        }

        /// <summary>
        /// Team attribute
        /// </summary>
        [JsonIgnore]
        public string? Team
        {
            get => GetAttribute(TraitType.Team);
            set => SetAttribute(TraitType.Team, value);
        }

        /// <summary>
        /// Computer name
        /// </summary>
        [JsonIgnore]
        public string? Computer
        {
            get => GetAttribute(TraitType.Computer);
            set => SetAttribute(TraitType.Computer, value);
        }

        /// <summary>
        /// TestId attribute
        /// </summary>
        [JsonIgnore]
        public string? TestId
        {
            get => GetAttribute(TraitType.TestId);
            set => SetAttribute(TraitType.TestId, value);
        }

        /// <summary>
        /// Tag attribute
        /// </summary>
        [JsonIgnore]
        public string? Tag
        {
            get => GetAttribute(TraitType.Tag);
            set => SetAttribute(TraitType.Tag, value);
        }

        /// <summary>
        /// Component
        /// </summary>
        [JsonIgnore]
        public string? Component
        {
            get => GetAttribute(TraitType.Component);
            set => SetAttribute(TraitType.Component, value);
        }

        /// <summary>
        /// Feature
        /// </summary>
        [JsonIgnore]
        public string? Feature
        {
            get => GetAttribute(TraitType.Feature);
            set => SetAttribute(TraitType.Feature, value);
        }

        /// <summary>
        /// Priority
        /// </summary>
        [JsonIgnore]
        public string? TestPriority
        {
            get => GetAttribute(TraitType.TestPriority);
            set => SetAttribute(TraitType.TestPriority, value);
        }

        #endregion Convenience trait access properties

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

        /// <summary>
        /// Sets an attribute, overwriting any attributes with the same type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public void SetAttribute(TraitType type, int? value)
        {
            Traits.RemoveAll(x => x.Type == type);
            if (value is not null)
            {
                Traits.Add(new TestTrait(type, value.Value.ToString()));
            }
        }

        /// <summary>
        /// Sets an attribute, overwriting any attributes with the same type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public void SetAttribute(TraitType type, DateTimeOffset? value)
        {
            Traits.RemoveAll(x => x.Type == type);
            if (value is not null)
            {
                Traits.Add(new TestTrait(type, value.Value.ToString("O")));
            }
        }

        /// <summary>
        /// Sets an attribute, overwriting any attributes with the same type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public void SetAttribute(TraitType type, TimeSpan? value)
        {
            Traits.RemoveAll(x => x.Type == type);
            if (value is not null)
            {
                Traits.Add(new TestTrait(type, value.Value.ToString()));
            }
        }

        /// <summary>
        /// Sets an attribute, overwriting any attributes with the same type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public void SetAttribute(TraitType type, string? value)
        {
            Traits.RemoveAll(x => x.Type == type);
            if (value is not null)
            {
                Traits.Add(new TestTrait(type, value));
            }
        }

        /// <summary>
        /// Gets an attribute of the specified type.
        /// If there are multiple only the first will be returned.
        /// If there are none null will be returned
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string? GetAttribute(TraitType type)
        {
            foreach (var attr in Traits
                .Where(x => x.Type == type))
            {
                return attr.Value;
            }
            return default;
        }
        
        /// <summary>
        /// Gets an attribute of the specified type.
        /// If there are multiple only the first will be returned.
        /// If there are none null will be returned
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets an attribute of the specified type.
        /// If there are multiple only the first will be returned.
        /// If there are none null will be returned
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets an attribute of the specified type.
        /// If there are multiple only the first will be returned.
        /// If there are none null will be returned
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns true if there are atleast one attribute of the specified type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool HasAttribute(TraitType type)
        {
            return Traits.Where(x => x.Type == type).Any();
        }
    }
}
