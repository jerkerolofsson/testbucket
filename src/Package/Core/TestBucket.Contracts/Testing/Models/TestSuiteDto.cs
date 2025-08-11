


using TestBucket.Contracts.Comments;

namespace TestBucket.Formats.Dtos
{
    public class TestSuiteDto
    {
        public long Id { get; set; }

        /// <summary>
        /// Traits for the test suite
        /// </summary>
        public TestTraitCollection? Traits { get; set; }

        /// <summary>
        /// Name of the test suite
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Description/readme
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Slug for the test suite
        /// </summary>
        public string? Slug { get; set; }

        /// <summary>
        /// Slug for the team
        /// </summary>
        public string? TeamSlug { get; set; }

        /// <summary>
        /// Slug for the project
        /// </summary>
        public string? ProjectSlug { get; set; }

        /// <summary>
        /// Test suite variables
        /// </summary>
        public Dictionary<string, string>? Variables { get; set; }

        /// <summary>
        /// CI/CD system name
        /// </summary>
        public string? CiCdSystem { get; set; }

        /// <summary>
        /// ID of the external system that this test suite is associated with, e.g. a Jira ID or GitHub ID
        /// </summary>
        public long? ExternalSystemId { get; set; }

        /// <summary>
        /// Gets or sets the default reference name used for CI/CD operations.
        /// </summary>
        public string? DefaultCiCdRef { get; set; }

        /// <summary>
        /// Required resources/accounts
        /// </summary>
        public List<TestCaseDependency>? Dependencies { get; set; }

        /// <summary>
        /// If true, pipelines started from an outside source (e.g. regular CI pipeline) will be indexed and runs added
        /// </summary>
        public bool? AddPipelinesStartedFromOutside { get; set; }

        /// <summary>
        /// Comments
        /// </summary>
        public List<CommentDto>? Comments { get; set; }
    }
}
