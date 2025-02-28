namespace TestBucket.Domain.Testing.Formats.Dtos
{
    public class TestCaseRunDto : TestTraitCollection
    {
        /// <summary>
        /// Passed/Failed etc
        /// </summary>
        public TestResult? Result
        {
            get
            {
                if (!HasAttribute(TestTraitType.TestResult))
                {
                    return TestResult.NoRun;
                }
                var intResult = GetAttributeAsInt32(TestTraitType.TestResult);
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
                    SetAttribute(TestTraitType.TestResult, (string?)null);
                }
                else
                {
                    SetAttribute(TestTraitType.TestResult, (int)value);
                }
            }
        }
        public TimeSpan? Duration
        {
            get => GetAttributeAsTimeSpan(TestTraitType.Duration);
            set => SetAttribute(TestTraitType.Duration, value);
        }

        /// <summary>
        /// Message attribute (failure message, skipped message etc..)
        /// </summary>
        public string? Message
        {
            get => GetAttribute(TestTraitType.Message);
            set => SetAttribute(TestTraitType.Message, value);
        }

        /// <summary>
        /// Failure type (could be exception name)
        /// </summary>
        public string? FailureType
        {
            get => GetAttribute(TestTraitType.FailureType);
            set => SetAttribute(TestTraitType.FailureType, value);
        }

        /// <summary>
        /// CallStack attribute
        /// </summary>
        public string? CallStack
        {
            get => GetAttribute(TestTraitType.CallStack);
            set => SetAttribute(TestTraitType.CallStack, value);
        }

        /// <summary>
        /// Line attribute
        /// </summary>
        public int? Line
        {
            get => GetAttributeAsInt32(TestTraitType.Line);
            set => SetAttribute(TestTraitType.Line, value);
        }


        /// <summary>
        /// ClassName attribute
        /// </summary>
        public string? ClassName
        {
            get => GetAttribute(TestTraitType.ClassName);
            set => SetAttribute(TestTraitType.ClassName, value);
        }

        /// <summary>
        /// Method attribute
        /// </summary>
        public string? Method
        {
            get => GetAttribute(TestTraitType.Method);
            set => SetAttribute(TestTraitType.Method, value);
        }
    }
}
