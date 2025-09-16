using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Requirements.Types;

/// <summary>
/// Default requirement specification types
/// </summary>
public class RequirementSpecificationTypes
{
    public static string[] AllTypes => [WorkItems, Roadmap, Documentation, SRS, PRS, FRS, BRS, RequirementSpecification, Other];

    /// <summary>
    /// General requirement
    /// </summary>
    public const string Documentation = "Documentation";

    /// <summary>
    /// PRS
    /// </summary>
    public const string PRS = "Product Requirement Specification (PRS)";

    /// <summary>
    /// SRS
    /// </summary>
    public const string SRS = "Software Requirement Specification (SRS)";

    /// <summary>
    /// FRS
    /// </summary>
    public const string FRS = "Functional Requirement Specification (FRS)";

    /// <summary>
    /// BRS
    /// </summary>
    public const string BRS = "Business Requirement Specification (BRS)";

    /// <summary>
    /// Requirement Specification
    /// </summary>
    public const string RequirementSpecification = "Requirement Specification";

    /// <summary>
    /// Collection for tasks, epics,..
    /// </summary>
    public const string WorkItems = "Work Items";

    /// <summary>
    /// Project roadmap
    /// </summary>
    public const string Roadmap = "Roadmap";

    /// <summary>
    /// Other
    /// </summary>
    public const string Other = "Other";
}
