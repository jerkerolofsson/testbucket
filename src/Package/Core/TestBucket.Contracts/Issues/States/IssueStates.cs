using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Issues.States;

/// <summary>
/// Default issue states
/// </summary>
public class IssueStates
{
    public static string[] AllStates => [Open, Triage, Triaged, Accepted, InProgress, Reviewed, Completed, Closed, Delivered, Canceled, Rejected];
    /// <summary>
    /// A new issue
    /// </summary>
    public const string Rejected = nameof(MappedIssueState.Rejected);


    /// <summary>
    /// A new issue
    /// </summary>
    public const string Open = nameof(MappedIssueState.Open);

    /// <summary>
    /// An issue which is waiting for triage
    /// </summary>
    public const string Triage = nameof(MappedIssueState.Triage);

    /// <summary>
    /// An issue which has been triaged but is not accepted yet
    /// </summary>
    public const string Triaged = nameof(MappedIssueState.Triaged);

    /// <summary>
    /// Accepted by a team
    /// </summary>
    public const string Accepted = nameof(MappedIssueState.Accepted);

    /// <summary>
    /// Assigned to a user
    /// </summary>
    public const string Assigned = nameof(MappedIssueState.Assigned);

    /// <summary>
    /// Assigned to a user, the user is working on it
    /// </summary>
    public const string InProgress = nameof(MappedIssueState.InProgress);

    /// <summary>
    /// Assigned for review
    /// </summary>
    public const string InReview = nameof(MappedIssueState.InReview);

    /// <summary>
    /// Solution is Reviewed
    /// </summary>
    public const string Reviewed = nameof(MappedIssueState.Reviewed);

    /// <summary>
    /// Completed
    /// </summary>
    public const string Completed = nameof(MappedIssueState.Completed);

    /// <summary>
    /// Delivered
    /// </summary>
    public const string Delivered = nameof(MappedIssueState.Delivered);

    /// <summary>
    /// Closed
    /// </summary>
    public const string Closed = nameof(MappedIssueState.Closed);

    /// <summary>
    /// Canceled
    /// </summary>
    public const string Canceled = nameof(MappedIssueState.Canceled);

    /// <summary>
    /// Other
    /// </summary>
    public const string Other = nameof(MappedIssueState.Other);
}
