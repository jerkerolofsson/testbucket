
namespace TestBucket.Formats.Dtos
{
    public class TestCaseRunDto : TestTraitCollection, ITestAttachmentSource
    {
        /// <summary>
        /// Passed/Failed etc
        /// </summary>
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
        /// Attachments
        /// </summary>
        public List<AttachmentDto> Attachments { get; set; } = [];

        /// <summary>
        /// Execution duration
        /// </summary>
        public TimeSpan? Duration
        {
            get => GetAttributeAsTimeSpan(TraitType.Duration);
            set => SetAttribute(TraitType.Duration, value);
        }

        /// <summary>
        /// Description of entity
        /// </summary>
        public string? Description
        {
            get => GetAttribute(TraitType.TestDescription);
            set => SetAttribute(TraitType.TestDescription, value);
        }

        /// <summary>
        /// Browser used
        /// </summary>
        public string? Browser
        {
            get => GetAttribute(TraitType.Browser);
            set => SetAttribute(TraitType.Browser, value);
        }

        /// <summary>
        /// Message attribute (failure message, skipped message etc..)
        /// </summary>
        public string? Message
        {
            get => GetAttribute(TraitType.FailureMessage);
            set => SetAttribute(TraitType.FailureMessage, value);
        }

        /// <summary>
        /// Failure type (could be exception name)
        /// </summary>
        public string? FailureType
        {
            get => GetAttribute(TraitType.FailureType);
            set => SetAttribute(TraitType.FailureType, value);
        }

        /// <summary>
        /// CallStack attribute
        /// </summary>
        public string? CallStack
        {
            get => GetAttribute(TraitType.CallStack);
            set => SetAttribute(TraitType.CallStack, value);
        }

        /// <summary>
        /// Line attribute
        /// </summary>
        public int? Line
        {
            get => GetAttributeAsInt32(TraitType.Line);
            set => SetAttribute(TraitType.Line, value);
        }


        /// <summary>
        /// ClassName attribute
        /// </summary>
        public string? ClassName
        {
            get => GetAttribute(TraitType.ClassName);
            set => SetAttribute(TraitType.ClassName, value);
        }

        /// <summary>
        /// Method attribute
        /// </summary>
        public string? Method
        {
            get => GetAttribute(TraitType.Method);
            set => SetAttribute(TraitType.Method, value);
        }

        /// <summary>
        /// Folders
        /// </summary>
        public string[] Folders { get; set; } = [];
    }
}
