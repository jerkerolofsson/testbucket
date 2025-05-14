using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Issues.Types;

/// <summary>
/// Default issue types
/// </summary>
public class IssueTypes
{
    public static string[] AllTypes => [Issue, Enhancement, Incident, Question, Other];

    /// <summary>
    /// An issue
    /// </summary>
    public const string Issue = nameof(MappedIssueType.Issue);

    /// <summary>
    /// An enhancement
    /// </summary>
    public const string Enhancement = nameof(MappedIssueType.Enhancement);

    /// <summary>
    /// An Incident
    /// </summary>
    public const string Incident = nameof(MappedIssueType.Incident);

    /// <summary>
    /// A question
    /// </summary>
    public const string Question = nameof(MappedIssueType.Question);

    /// <summary>
    /// Other, not mapped
    /// </summary>

    public const string Other = nameof(MappedIssueType.Other);
}
