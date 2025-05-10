using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Comments.Models;
public class Comment : ProjectEntity
{
    public long Id { get; set; }

    /// <summary>
    /// Comment, in markdown format
    /// </summary>
    public string? Markdown { get; set; }

    /// <summary>
    /// A special action which is logged, for example "approved"
    /// </summary>
    public string? LoggedAction { get; set; }

    /// <summary>
    /// Argument related to the action, for example a specific field value
    /// </summary>
    public string? LoggedActionArgument { get; set; }

    // Navigation

    public long? RequirementId { get; set; }
    public long? RequirementSpecificationId { get; set; }
    public long? TestCaseId { get; set; }
    public long? TestRunId { get; set; }
    public long? TestCaseRunId { get; set; }
    public long? TestSuiteId { get; set; }

    public Requirement? Requirement { get; set; }
    public TestCase? TestCase { get; set; }
    public TestCaseRun? TestCaseRun { get; set; }
    public TestRun? TestRun { get; set; }
    public TestSuite? TestSuite { get; set; }
    public RequirementSpecification? RequirementSpecification { get; set; }
}
