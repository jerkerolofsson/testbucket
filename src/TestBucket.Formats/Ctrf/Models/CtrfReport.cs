using System.Text.Json.Serialization;

namespace TestBucket.Formats.Ctrf.Models
{

    public class CtrfReport<TResultsExtra, TEnvironmentExtra, TTestsExtra>
        where TResultsExtra : ResultsExtra
        where TEnvironmentExtra : EnvironmentExtra 
        where TTestsExtra : TestExtra
    {
        /// <summary>
        /// Report format
        /// </summary>
        [JsonPropertyName("reportFormat")] 
        public string? ReportFormat { get; set; }
        
        [JsonPropertyName("specVersion")] 
        public string? SpecVersion { get; set; }

        /// <summary>
        /// Test results
        /// </summary>
        [JsonPropertyName("results")]
        public required CtrfResults<TResultsExtra, TEnvironmentExtra, TTestsExtra> Results { get; set; }
    }

    public class CtrfResults<TResultsExtra,TEnvironmentExtra, TTestsExtra>
        where TResultsExtra : ResultsExtra
        where TEnvironmentExtra : EnvironmentExtra
        where TTestsExtra : TestExtra
    {
        [JsonPropertyName("tool")] 
        public required CtrfTool Tool { get; set; }
        
        [JsonPropertyName("summary")] 
        public required CtrfSummary Summary { get; set; }
        
        [JsonPropertyName("tests")] 
        public required CtrfTest<TTestsExtra>[] Tests { get; set; }
        
        [JsonPropertyName("environment")] 
        public CtrfEnvironment<TEnvironmentExtra>? Environment { get; set; }

        [JsonPropertyName("extra")]
        public TResultsExtra? Extra { get; set; }

    }

    public class CtrfEnvironment<TEnvironmentExtra> where TEnvironmentExtra : EnvironmentExtra
    {
        /// <summary>
        /// The name of the report
        /// </summary>
        public string? ReportName { get; set; }

        /// <summary>
        /// Application being tested
        /// </summary>
        public string? AppName { get; set; }

        /// <summary>
        /// Application version being tested
        /// </summary>
        public string? AppVersion { get; set; }

        /// <summary>
        /// The name of the build (e.g., feature branch name).
        /// </summary>
        public string? BuildName { get; set; }

        /// <summary>
        /// The URL to the build in the CI/CD system.
        /// </summary>
        public string? BuildUrl { get; set; }

        /// <summary>
        /// The name of the repository where the code is hosted.
        /// </summary>
        public string? RepositoryName { get; set; }

        /// <summary>
        /// The name of the branch from which the tests were run.
        /// </summary>
        public string? BranchName { get; set; }

        /// <summary>
        /// The operating system platform (e.g., Windows, Linux).
        /// </summary>
        public string? OsPlatform { get; set; }

        /// <summary>
        /// The release version of the operating system.
        /// </summary>
        public string? OsRelease { get; set; }

        /// <summary>
        /// The version number of the operating system.
        /// </summary>
        public string? OsVersion { get; set; }

        /// <summary>
        /// he environment where the tests were run (e.g., staging, production).
        /// </summary>
        public string? TestEnvironment { get; set; }

        /// <summary>
        /// The commit hash
        /// </summary>
        public string? Commit { get; set; }

        /// <summary>
        /// custom data relevant to the environment
        /// </summary>
        public TEnvironmentExtra? Extra { get; set; }
    }

    public class CtrfTool
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }
        
        [JsonPropertyName("version")] 
        public string? Version { get; set; }
    }

    public class CtrfSummary
    {
        [JsonPropertyName("tests")]
        public int Tests { get; set; }

        [JsonPropertyName("passed")]
        public int Passed { get; set; }
        
        [JsonPropertyName("failed")] 
        public int Failed { get; set; }
        
        [JsonPropertyName("pending")] 
        public int Pending { get; set; }
        
        [JsonPropertyName("skipped")] 
        public int Skipped { get; set; }
        
        [JsonPropertyName("other")] 
        public int Other { get; set; }

        [JsonPropertyName("suites")] 
        public int Suites { get; set; }

        /// <summary>
        /// Start timestamps, millisecond unix time
        /// </summary>
        [JsonPropertyName("start")] 
        public long Start { get; set; }

        /// <summary>
        /// End timestamp, millisecond unix time
        /// </summary>
        [JsonPropertyName("stop")] 
        public long Stop { get; set; }
    }

    public class CtrfTest<TTestExtra> where TTestExtra : TestExtra
    {
        [JsonPropertyName("name")] 
        public required string Name { get; set; }

        /// <summary>
        /// https://ctrf.io/docs/specification/status
        /// passed, failed, skipped, pending, or other
        /// </summary>
        [JsonPropertyName("status")] 
        public required string Status { get; set; }

        /// <summary>
        /// Duration, in milliseconds
        /// </summary>
        [JsonPropertyName("duration")] public required long Duration { get; set; }

        /// <summary>
        /// Start timestamps, millisecond unix time
        /// </summary>
        [JsonPropertyName("start")]
        public long Start { get; set; }

        /// <summary>
        /// End timestamp, millisecond unix time
        /// </summary>
        [JsonPropertyName("stop")]
        public long Stop { get; set; }
        
        [JsonPropertyName("suite")] 
        public string? Suite { get; set; }

        [JsonPropertyName("rawStatus")] 
        public string? RawStatus { get; set; }

        [JsonPropertyName("line")]
        public int? Line { get; set; }

        [JsonPropertyName("tags")] 
        public string[]? Tags { get; set; }

        /// <summary>
        /// Example: "Integration"
        /// </summary>
        [JsonPropertyName("type")] 
        public string? Type { get; set; }
        
        [JsonPropertyName("filePath")] 
        public string? FilePath { get; set; }
        
        [JsonPropertyName("retries")] 
        public int? Retries { get; set; }
        
        [JsonPropertyName("flaky")] 
        public bool? Flaky { get; set; }

        [JsonPropertyName("browser")] 
        public string? Browser { get; set; }

        [JsonPropertyName("extra")] 
        public TTestExtra? Extra { get; set; }

        [JsonPropertyName("screenshot")]
        public string? Screenshot { get; set; }

        [JsonPropertyName("message")] 
        public string? Message { get; set; }
        
        [JsonPropertyName("trace")]
        public string? Trace { get; set; }
    }

    public class CtrfTraits : Dictionary<string, string[]>
    {
    }


    public class Extra
    {
    }

    public class SummaryExtra : Extra { }
    public class ToolExtra : Extra { }
    public class ResultsExtra : Extra { }
    public class EnvironmentExtra : Extra { }
    public class TestExtra : Extra { }




}
