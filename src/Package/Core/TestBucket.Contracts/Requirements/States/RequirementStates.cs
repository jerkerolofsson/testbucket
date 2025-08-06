using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Requirements.States;

/// <summary>
/// Default requirement states
/// </summary>
public class RequirementStates
{
    public static string[] DefaultStateNames => [Draft, Accepted, InProgress, Reviewed, Completed, Delivered, Canceled];

    /// <summary>
    /// A new requirement
    /// </summary>
    public const string Draft = nameof(MappedRequirementState.Draft);

    /// <summary>
    /// Accepted for a product
    /// </summary>
    public const string Accepted = nameof(MappedRequirementState.Accepted);

    /// <summary>
    /// Assigned to a user
    /// </summary>
    public const string Assigned = nameof(MappedRequirementState.Assigned);

    /// <summary>
    /// Assigned to a user, the user is working on it
    /// </summary>
    public const string InProgress = nameof(MappedRequirementState.InProgress);

    /// <summary>
    /// Reviewed
    /// </summary>
    public const string Reviewed = nameof(MappedRequirementState.Reviewed);

    /// <summary>
    /// Completed
    /// </summary>
    public const string Completed = nameof(MappedRequirementState.Completed);

    /// <summary>
    /// Delivered
    /// </summary>
    public const string Delivered = nameof(MappedRequirementState.Delivered);

    /// <summary>
    /// Canceled
    /// </summary>
    public const string Canceled = nameof(MappedRequirementState.Canceled);
}
