using System.Text.Json.Serialization;

using TestBucket.Formats.Abstractions;
using TestBucket.Traits.Core;

namespace TestBucket.Formats.Dtos
{
    /// <summary>
    /// Contains metadata and results related to one executed test case
    /// </summary>
    public class TestCaseRunDto : TestTraitCollection, ITestAttachmentSource
    {
        #region Convenience trait access properties
        /// <summary>
        /// Passed/Failed etc
        /// </summary>
        [JsonIgnore]
        public TestResult? Result
        {
            get
            {
                if (!HasAttribute(TraitType.TestResult))
                {
                    return TestResult.NoRun;
                }
                var intResult = GetAttributeAsInt32(TraitType.TestResult);
                if (intResult is null)
                {
                    return TestResult.NoRun;
                }
                return (TestResult)intResult;
            }
            set
            {
                if (value is null)
                {
                    SetAttribute(TraitType.TestResult, (string?)null);
                }
                else
                {
                    SetAttribute(TraitType.TestResult, (int)value);
                }
            }
        }

        /// <summary>
        /// Execution duration
        /// </summary>
        [JsonIgnore]
        public TimeSpan? Duration
        {
            get => GetAttributeAsTimeSpan(TraitType.Duration);
            set => SetAttribute(TraitType.Duration, value);
        }

        /// <summary>
        /// Description of entity
        /// </summary>
        [JsonIgnore]
        public string? Description
        {
            get => GetAttribute(TraitType.TestDescription);
            set => SetAttribute(TraitType.TestDescription, value);
        }

        /// <summary>
        /// Browser used
        /// </summary>
        [JsonIgnore]
        public string? Browser
        {
            get => GetAttribute(TraitType.Browser);
            set => SetAttribute(TraitType.Browser, value);
        }

        /// <summary>
        /// Message attribute (failure message, skipped message etc..)
        /// </summary>
        [JsonIgnore]
        public string? Message
        {
            get => GetAttribute(TraitType.FailureMessage);
            set => SetAttribute(TraitType.FailureMessage, value);
        }

        /// <summary>
        /// Failure type (could be exception name)
        /// </summary>
        [JsonIgnore]
        public string? FailureType
        {
            get => GetAttribute(TraitType.FailureType);
            set => SetAttribute(TraitType.FailureType, value);
        }

        /// <summary>
        /// CallStack attribute
        /// </summary>
        [JsonIgnore]
        public string? CallStack
        {
            get => GetAttribute(TraitType.CallStack);
            set => SetAttribute(TraitType.CallStack, value);
        }

        /// <summary>
        /// Line attribute
        /// </summary>
        [JsonIgnore]
        public int? Line
        {
            get => GetAttributeAsInt32(TraitType.Line);
            set => SetAttribute(TraitType.Line, value);
        }


        /// <summary>
        /// ClassName attribute
        /// </summary>
        [JsonIgnore]
        public string? ClassName
        {
            get => GetAttribute(TraitType.ClassName);
            set => SetAttribute(TraitType.ClassName, value);
        }

        /// <summary>
        /// Method attribute
        /// </summary>
        [JsonIgnore]
        public string? Method
        {
            get => GetAttribute(TraitType.Method);
            set => SetAttribute(TraitType.Method, value);
        }
        #endregion Convenience trait access properties

        /// <summary>
        /// Attachments
        /// </summary>
        public List<AttachmentDto> Attachments { get; set; } = [];

        /// <summary>
        /// Folder path
        /// </summary>
        public string[] Folders { get; set; } = [];

        /// <summary>
        /// Numerical results
        /// </summary>
        public List<NumericalResultDto> NumericalResults { get; set; } = [];
    }
}
