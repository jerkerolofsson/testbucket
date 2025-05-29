using Mediator;

using TestBucket.Domain.Code.Events;
using TestBucket.Domain.Code.Features.ConventionalCommits.Models;
using TestBucket.Domain.Code.Features.ConventionalCommits.Parser;
using TestBucket.Domain.Code.Services;
using TestBucket.Domain.Issues;
using TestBucket.Domain.Issues.Requests;

namespace TestBucket.Domain.Code.Features.ConventionalCommits;

/// <summary>
/// Parses the commit message and tries to identify fixes, features etc from a commit message
/// following the conventional commit standard.
/// </summary>
internal class ExtractDataFromCommitByConvention : INotificationHandler<CommitAddedEvent>
{
    private readonly ICommitManager _commitManager;
    private readonly IIssueManager _issueManager;
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtractDataFromCommitByConvention"/> class.
    /// </summary>
    /// <param name="commitManager">The commit manager used to update commit data.</param>
    /// <param name="issueManager">The issue manager used to find and update issues.</param>
    /// <param name="mediator">The mediator for sending requests.</param>
    public ExtractDataFromCommitByConvention(ICommitManager commitManager, IIssueManager issueManager, IMediator mediator)
    {
        _commitManager = commitManager;
        _issueManager = issueManager;
        _mediator = mediator;
    }

    /// <summary>
    /// Handles the <see cref="CommitAddedEvent"/> by parsing the commit message and updating commit and issue data as needed.
    /// </summary>
    /// <param name="notification">The commit added event notification.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    public async ValueTask Handle(CommitAddedEvent notification, CancellationToken cancellationToken)
    {
        var commit = notification.Commit;
        var principal = notification.Principal;

        if (!string.IsNullOrEmpty(commit.Message))
        {
            // Extract conventional commit attributes from the commit message
            ConventionalCommitParser parser = new();
            var conventionalCommit = parser.Parse(commit.Message);

            bool changed = false;

            if (!string.IsNullOrEmpty(conventionalCommit.Description))
            {
                commit.ShortDescription = conventionalCommit.Description;
                changed = true;
            }
            if (!string.IsNullOrEmpty(conventionalCommit.LongerDescription))
            {
                commit.LongDescription = conventionalCommit.LongerDescription;
                changed = true;
            }

            // Scan both type-line and footer and process special type identifiers
            foreach (ConventionalCommitType type in conventionalCommit.GetTypes())
            {
                changed = ExtractFeature(commit, changed, type);
                changed = ExtractReferences(commit, changed, type);
                changed = await ExtractFixesAsync(commit, principal, changed, type);
            }

            if (changed)
            {
                await _commitManager.UpdateCommitAsync(principal, commit);
            }
        }
    }

    /// <summary>
    /// Extracts and processes "Fixes" types from the commit, closing issues if necessary.
    /// </summary>
    /// <param name="commit">The commit being processed.</param>
    /// <param name="principal">The user principal.</param>
    /// <param name="changed">Indicates if the commit was changed.</param>
    /// <param name="type">The conventional commit type.</param>
    /// <returns>A <see cref="Task{Boolean}"/> indicating if the commit was changed.</returns>
    private async Task<bool> ExtractFixesAsync(Code.Models.Commit commit, ClaimsPrincipal principal, bool changed, ConventionalCommitType type)
    {
        if (type.Description is not null && type.Type.Equals("Fixes", StringComparison.InvariantCultureIgnoreCase))
        {
            commit.Fixes ??= new();
            if (!commit.Fixes.Contains(type.Description))
            {
                // Close the issue
                if (commit.TestProjectId is not null)
                {
                    await CloseIssueAsync(principal, commit.TestProjectId.Value, type.Description, commit.Sha);
                }

                commit.Fixes.Add(type.Description);
                changed = true;
            }
        }

        return changed;
    }

    /// <summary>
    /// Extracts and processes "Ref" types from the commit, adding references as needed.
    /// </summary>
    /// <param name="commit">The commit being processed.</param>
    /// <param name="changed">Indicates if the commit was changed.</param>
    /// <param name="type">The conventional commit type.</param>
    /// <returns>True if the commit was changed; otherwise, false.</returns>
    private static bool ExtractReferences(Code.Models.Commit commit, bool changed, ConventionalCommitType type)
    {
        if (type.Description is not null && type.Type.Equals("Ref", StringComparison.InvariantCultureIgnoreCase))
        {
            commit.References ??= new();
            if (!commit.References.Contains(type.Description))
            {
                commit.References.Add(type.Description);
                changed = true;
            }
        }

        return changed;
    }

    /// <summary>
    /// Extracts and processes "feat" types from the commit, adding feature names as needed.
    /// </summary>
    /// <param name="commit">The commit being processed.</param>
    /// <param name="changed">Indicates if the commit was changed.</param>
    /// <param name="type">The conventional commit type.</param>
    /// <returns>True if the commit was changed; otherwise, false.</returns>
    private static bool ExtractFeature(Code.Models.Commit commit, bool changed, ConventionalCommitType type)
    {
        if (type.Scope is not null && type.Type.Equals("feat", StringComparison.InvariantCultureIgnoreCase))
        {
            commit.FeatureNames ??= new();
            if (!commit.FeatureNames.Contains(type.Scope))
            {
                commit.FeatureNames.Add(type.Scope);
                changed = true;
            }
        }

        return changed;
    }

    /// <summary>
    /// Closes an issue associated with a commit if it exists.
    /// </summary>
    /// <param name="principal">The user principal.</param>
    /// <param name="testProjectId">The test project ID.</param>
    /// <param name="issueIdentifier">The issue identifier.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private async Task CloseIssueAsync(ClaimsPrincipal principal, long testProjectId, string issueIdentifier, string commitSha)
    {
        var issue = await _issueManager.FindLocalIssueAsync(principal, testProjectId, issueIdentifier);
        if (issue is not null)
        {
            // Link commit

            await _mediator.Send(new CloseIssueRequest(principal, issue, commitSha));
        }
    }
}