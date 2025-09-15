using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Comments;
internal class CommentsManager : ICommentsManager
{
    private readonly ICommentRepository _repository;
    private readonly TimeProvider _timeProvider;

    public CommentsManager(ICommentRepository repository, TimeProvider timeProvider)
    {
        _repository = repository;
        _timeProvider = timeProvider;
    }

    public async Task AddCommentAsync(ClaimsPrincipal principal, Comment comment)
    {
        comment.Modified = comment.Created = _timeProvider.GetUtcNow();
        comment.ModifiedBy = comment.CreatedBy = principal.Identity?.Name ?? throw new UnauthorizedAccessException("Invalid identity");

        ThrowIfNoPermission(principal, comment, PermissionLevel.Write);
        comment.TenantId = principal.GetTenantIdOrThrow();
        await _repository.AddAsync(comment);
    }

    public async Task UpdateCommentAsync(ClaimsPrincipal principal, Comment comment)
    {
        comment.Modified = _timeProvider.GetUtcNow();
        comment.ModifiedBy = principal.Identity?.Name ?? throw new UnauthorizedAccessException("Invalid identity");

        ThrowIfNoPermission(principal, comment, PermissionLevel.Write);
        principal.ThrowIfEntityTenantIsDifferent(comment);
        await _repository.UpdateAsync(comment);
    }

    public async Task DeleteCommentAsync(ClaimsPrincipal principal, Comment comment)
    {
        ThrowIfNoPermission(principal, comment, PermissionLevel.Delete);
        principal.ThrowIfEntityTenantIsDifferent(comment);
        await _repository.DeleteAsync(comment);
    }

    public static void ThrowIfNoPermission(ClaimsPrincipal principal, Comment comment, PermissionLevel level)
    {
        PermissionEntityType type = PermissionEntityType.Requirement;
        if (comment.RequirementId is not null)
        {
            type = PermissionEntityType.Requirement;
        }
        else if (comment.RequirementSpecificationId is not null)
        {
            type = PermissionEntityType.RequirementSpecification;
        }
        else if (comment.RequirementSpecificationId is not null)
        {
            type = PermissionEntityType.RequirementSpecification;
        }
        else if (comment.TestCaseId is not null)
        {
            type = PermissionEntityType.TestCase;
        }
        else if (comment.TestRunId is not null)
        {
            type = PermissionEntityType.TestRun;
        }
        else if (comment.TestCaseRunId is not null)
        {
            type = PermissionEntityType.TestCaseRun;
        }
        else if (comment.TestSuiteId is not null)
        {
            type = PermissionEntityType.TestSuite;
        }
        principal.ThrowIfNoPermission(type, level);
    }
}
