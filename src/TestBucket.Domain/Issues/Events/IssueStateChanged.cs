using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.Issues.Models;

namespace TestBucket.Domain.Issues.Events;

public record class IssueStateChangedNotification(ClaimsPrincipal Principal, LocalIssue Issue, string? OldState) : INotification;

