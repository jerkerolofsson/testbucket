using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.Issues.Models;

namespace TestBucket.Domain.Issues.Events;

/// <summary>
/// Event raised when the state of an issue is changing, before it is saved
/// </summary>
/// <param name="Principal"></param>
/// <param name="Issue"></param>
/// <param name="OldState"></param>
public record class IssueStateChangingNotification(ClaimsPrincipal Principal, LocalIssue Issue, string? OldState) : INotification;

