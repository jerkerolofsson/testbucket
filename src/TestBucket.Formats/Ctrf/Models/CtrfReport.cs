using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Formats.Ctrf.Models
{

    public class CtrfReport<TEnvironmentExtra, TTestsExtra> 
        where TEnvironmentExtra : EnvironmentExtra 
        where TTestsExtra : TestExtra
    {
        /// <summary>
        /// Report format
        /// </summary>
        public string? ReportFormat { get; set; }
        public string? SpecVersion { get; set; }

        /// <summary>
        /// Test results
        /// </summary>
        public required CtrfResults<TEnvironmentExtra, TTestsExtra> Results { get; set; }
    }

    public class CtrfResults<TEnvironmentExtra, TTestsExtra>
        where TEnvironmentExtra : EnvironmentExtra
        where TTestsExtra : TestExtra
    {
        public required CtrfTool Tool { get; set; }
        public required CtrfSummary Summary { get; set; }
        public required CtrfTest<TTestsExtra>[] Tests { get; set; }
        public CtrfEnvironment<TEnvironmentExtra>? Environment { get; set; }
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
        public TResultsExtra? Extra { get; set; }
    }

    public class CtrfTool
    {
        public required string Name { get; set; }
        public string? Version { get; set; }
    }

    public class CtrfSummary
    {
        public int Tests { get; set; }
        public int Passed { get; set; }
        public int Failed { get; set; }
        public int Pending { get; set; }
        public int Skipped { get; set; }
        public int Other { get; set; }
        public int Suites { get; set; }

        /// <summary>
        /// Start timestamps, epochs
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// End timestamp, epochs
        /// </summary>
        public int Stop { get; set; }
    }

    public class CtrfTest<TTestExtra> where TTestExtra : TestExtra
    {
        public required string Name { get; set; }

        /// <summary>
        /// https://ctrf.io/docs/specification/status
        /// passed, failed, skipped, pending, or other
        /// </summary>
        public required string Status { get; set; }

        /// <summary>
        /// Duration, in milliseconds?
        /// </summary>
        public required int Duration { get; set; }
        public long Start { get; set; }
        public long Stop { get; set; }
        public string? Suite { get; set; }
        public string? RawStatus { get; set; }
        public string[]? Tags { get; set; }

        /// <summary>
        /// Example: "Integration"
        /// </summary>
        public string? Type { get; set; }
        public string? FilePath { get; set; }
        public int? Retries { get; set; }
        public bool? Flaky { get; set; }
        public string? Browser { get; set; }
        public TTestExtra? Extra { get; set; }
        public string? Screenshot { get; set; }
        public string? Message { get; set; }
        public string? Trace { get; set; }
    }

    public class CtrfTraits : Dictionary<string, string[]>
    {
    }


    public class Extra
    {
    }

    public class EnvironmentExtra : Extra { }
    public class TestExtra : Extra { }




}
