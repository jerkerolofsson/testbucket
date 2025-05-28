using Mediator;

using TestBucket.Domain.Code.Events;
using TestBucket.Domain.Code.Features.ConventionalCommits.Models;
using TestBucket.Domain.Code.Features.ConventionalCommits.Parser;
using TestBucket.Domain.Code.Services;

namespace TestBucket.Domain.Code.Features.ConventionalCommits;

/// <summary>
/// Parses the commit message and tries to identify fixes, features etc from  a commit message
/// following the conventional commit standard
/// </summary>
internal class ExtractDataFromCommitByConvention : INotificationHandler<CommitAddedEvent>
{
    private readonly ICommitManager _commitManager;

    public ExtractDataFromCommitByConvention(ICommitManager commitManager)
    {
        _commitManager = commitManager;
    }

    public async ValueTask Handle(CommitAddedEvent notification, CancellationToken cancellationToken)
    {
        var commit = notification.Commit;
        var principal = notification.Principal;

        if (!string.IsNullOrEmpty(commit.Message))
        {
            ConventionalCommitParser parser = new();
            var conventionalCommit = parser.Parse(commit.Message);

            bool changed = false;

            // Scan both type-line and footer
            foreach (ConventionalCommitType type in conventionalCommit.GetTypes())
            {
                if (!string.IsNullOrEmpty(type.Scope))
                {
                    // Extract feature
                    if (type.Type.Equals("feat", StringComparison.InvariantCultureIgnoreCase))
                    {
                        commit.FeatureNames ??= new();
                        if (!commit.FeatureNames.Contains(type.Scope))
                        {
                            commit.FeatureNames.Add(type.Scope);
                            changed = true;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(type.Description))
                {
                    // Fixes
                    if (type.Type.Equals("Fixes", StringComparison.InvariantCultureIgnoreCase))
                    {
                        commit.Fixes ??= new();
                        if (!commit.Fixes.Contains(type.Description))
                        {
                            commit.Fixes.Add(type.Description);
                            changed = true;
                        }
                    }
                }
            }

            if (changed)
            {
                await _commitManager.UpdateCommitAsync(principal, commit);
            }
        }
    }
}
