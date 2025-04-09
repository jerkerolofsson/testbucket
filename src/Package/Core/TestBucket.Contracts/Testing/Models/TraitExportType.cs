namespace TestBucket.Contracts.Testing.Models
{
    /// <summary>
    /// For formats that support both traits and properties, this defines how a value should be exported
    /// </summary>
    public enum TraitExportType
    {
        /// <summary>
        /// Exported on a static element, if supported, for example TestCase, not TestCaseRun
        /// </summary>
        Static = 0,

        /// <summary>
        /// Value is changing between runs, so should be exported on TestRun, TestCaseRun..
        /// </summary>
        Instance = 1
    }
}
