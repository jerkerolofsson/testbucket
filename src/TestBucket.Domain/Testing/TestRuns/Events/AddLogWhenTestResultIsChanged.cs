using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.Comments;

namespace TestBucket.Domain.Testing.TestRuns.Events;
internal class AddLogWhenTestResultIsChanged : INotificationHandler<TestCaseRunUpdatingNotification>
{
    private readonly ICommentsManager _comments;

    public AddLogWhenTestResultIsChanged(ICommentsManager comments)
    {
        _comments = comments;
    }

    public async ValueTask Handle(TestCaseRunUpdatingNotification notification, CancellationToken cancellationToken)
    {
        if(notification.New.Result != notification.Old.Result)
        {
            var testCaseRun = notification.New;

            var comment = new Comment
            {
                TestCaseRunId = testCaseRun.Id,
                TenantId = testCaseRun.TenantId,
                TestProjectId = testCaseRun.TestProjectId,
                LoggedAction = "result-changed",
                LoggedActionArgument = testCaseRun.Result.ToString(),
                LoggedActionIcon = TbIcons.BoldDuoTone.Check,
                LoggedActionColor = testCaseRun.Result switch
                {
                    TestResult.Failed => "red",
                    TestResult.Blocked => "yellow",
                    TestResult.Passed => "yellowgreen",
                    _ => null
                }
            };
            await _comments.AddCommentAsync(notification.Principal, comment);
        }
    }
}
