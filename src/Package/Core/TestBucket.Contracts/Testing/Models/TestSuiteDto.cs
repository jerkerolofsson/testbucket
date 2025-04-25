

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
        public Dictionary<string, string>? Variables { get; set; }
        public string? CiCdSystem { get; set; }
        public long? ExternalSystemId { get; set; }
        public string? DefaultCiCdRef { get; set; }
    }
}
